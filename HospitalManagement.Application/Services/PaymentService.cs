using HospitalManagement.Application.DTOs;
using HospitalManagement.Domain.Models;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services
{
    public class PaymentService
    {
        private readonly ApplicationDBContext _context;

        public PaymentService(ApplicationDBContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<PaymentResponseDto> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var bill = await _context.Bill
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(b => b.Id == dto.BillId);

            if (bill == null)
                throw new KeyNotFoundException($"Bill with ID {dto.BillId} not found");

            if (dto.Amount > bill.Amount)
                throw new InvalidOperationException("Payment amount cannot exceed bill amount");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BillId = dto.BillId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaidAt = DateTime.Now
            };

            _context.Payment.Add(payment);

            // Update bill paid status if full payment
            var totalPayments = await _context.Payment
                .Where(p => p.BillId == dto.BillId)
                .SumAsync(p => p.Amount);

            if (totalPayments + dto.Amount >= bill.Amount)
            {
                bill.IsPaid = true;
                _context.Bill.Update(bill);

                // When bill is paid, mark all related prescriptions as dispensed and update inventory
                await DispensePrescriptionsAndUpdateInventoryAsync(bill.PatientId);
            }

            await _context.SaveChangesAsync();

            return MapToResponseDto(payment);
        }

        // Helper method to dispense prescriptions and update inventory when bill is paid
        private async Task DispensePrescriptionsAndUpdateInventoryAsync(Guid patientId)
        {
            // Get all unpaid appointments for this patient
            var appointments = await _context.Appointment
                .Where(a => a.PatientId == patientId && a.AppointmentStatus != "Cancelled")
                .Include(a => a.Prescriptions)
                    .ThenInclude(p => p.PrescriptionItems)
                    .ThenInclude(pi => pi.Medicine)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                foreach (var prescription in appointment.Prescriptions.Where(p => !p.isDispensed))
                {
                    // Mark prescription as dispensed
                    prescription.isDispensed = true;
                    prescription.DispensedAt = DateTime.Now;

                    // Update inventory and medicine stock for each item
                    foreach (var item in prescription.PrescriptionItems)
                    {
                        // Create inventory log entry
                        var inventoryLog = new InventoryLogs
                        {
                            Id = Guid.NewGuid(),
                            MedicineId = item.MedicineId,
                            QuantityChanged = -item.Quantity, // Negative for dispensed
                            ActionType = "Dispensed",
                            Remarks = $"Dispensed for patient {appointment.Patient?.PatientName} - Prescription {prescription.Id}"
                        };

                        _context.InventoryLogs.Add(inventoryLog);

                        // Update medicine stock quantity
                        if (item.Medicine != null)
                        {
                            item.Medicine.StockQuantity -= item.Quantity;
                            item.Medicine.LastUpdated = DateTime.Now;
                            _context.Medicines.Update(item.Medicine);
                        }
                    }

                    _context.Prescriptions.Update(prescription);
                }
            }

            await _context.SaveChangesAsync();
        }

        // READ ALL
        public async Task<List<PaymentResponseDto>> GetAllPaymentsAsync()
        {
            var payments = await _context.Payment.ToListAsync();
            return payments.Select(MapToResponseDto).ToList();
        }

        // READ BY ID
        public async Task<PaymentResponseDto> GetPaymentByIdAsync(Guid id)
        {
            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found");

            return MapToResponseDto(payment);
        }

        // READ BY BILL ID
        public async Task<List<PaymentResponseDto>> GetPaymentsByBillIdAsync(Guid billId)
        {
            var payments = await _context.Payment
                .Where(p => p.BillId == billId)
                .ToListAsync();

            return payments.Select(MapToResponseDto).ToList();
        }

        // UPDATE
        public async Task<PaymentResponseDto> UpdatePaymentAsync(UpdatePaymentDto dto)
        {
            var payment = await _context.Payment.FindAsync(dto.Id);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {dto.Id} not found");

            payment.Amount = dto.Amount;
            payment.PaymentMethod = dto.PaymentMethod;

            _context.Payment.Update(payment);
            await _context.SaveChangesAsync();

            return MapToResponseDto(payment);
        }

        // DELETE
        public async Task<bool> DeletePaymentAsync(Guid id)
        {
            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found");

            _context.Payment.Remove(payment);
            await _context.SaveChangesAsync();

            return true;
        }

        // Helper method to map to DTO
        private PaymentResponseDto MapToResponseDto(Payment payment)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                BillId = payment.BillId,
                PaidAt = payment.PaidAt,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod
            };
        }
    }
}
