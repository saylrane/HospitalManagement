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
    public class PrescriptionService
    {
        private readonly ApplicationDBContext _context;

        public PrescriptionService(ApplicationDBContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<PrescriptionResponseDto> CreatePrescriptionAsync(CreatePrescriptionDto dto)
        {
            var appointment = await _context.Appointment.FindAsync(dto.AppointmentId);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {dto.AppointmentId} not found");

            var prescription = new Prescription
            {
                Id = Guid.NewGuid(),
                AppointmentId = dto.AppointmentId,
                DatePrescribed = DateTime.Now,
                Remark = dto.Remark,
                isDispensed = false,
                PrescriptionItems = new List<PrescriptionItems>()
            };

            // Add prescription items
            foreach (var item in dto.PrescriptionItems)
            {
                var medicine = await _context.Medicines.FindAsync(item.MedicineId);
                if (medicine == null)
                    throw new KeyNotFoundException($"Medicine with ID {item.MedicineId} not found");

                var prescriptionItem = new PrescriptionItems
                {
                    Id = Guid.NewGuid(),
                    PrescriptionId = prescription.Id,
                    MedicineId = item.MedicineId,
                    Quantity = item.Quantity,
                    Dosage = item.Dosage,
                    DurationDays = item.DurationDays
                };

                prescription.PrescriptionItems.Add(prescriptionItem);
            }

            _context.Prescriptions.Add(prescription);
            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(prescription);
        }

        // READ ALL
        public async Task<List<PrescriptionResponseDto>> GetAllPrescriptionsAsync()
        {
            var prescriptions = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .ThenInclude(pi => pi.Medicine)
                .ToListAsync();

            var dtos = new List<PrescriptionResponseDto>();
            foreach (var prescription in prescriptions)
            {
                dtos.Add(await MapToResponseDtoAsync(prescription));
            }
            return dtos;
        }

        // READ BY ID
        public async Task<PrescriptionResponseDto> GetPrescriptionByIdAsync(Guid id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .ThenInclude(pi => pi.Medicine)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                throw new KeyNotFoundException($"Prescription with ID {id} not found");

            return await MapToResponseDtoAsync(prescription);
        }

        // READ PRESCRIPTIONS BY APPOINTMENT
        public async Task<List<PrescriptionResponseDto>> GetPrescriptionsByAppointmentIdAsync(Guid appointmentId)
        {
            var prescriptions = await _context.Prescriptions
                .Where(p => p.AppointmentId == appointmentId)
                .Include(p => p.PrescriptionItems)
                .ThenInclude(pi => pi.Medicine)
                .ToListAsync();

            var dtos = new List<PrescriptionResponseDto>();
            foreach (var prescription in prescriptions)
            {
                dtos.Add(await MapToResponseDtoAsync(prescription));
            }
            return dtos;
        }

        // UPDATE
        public async Task<PrescriptionResponseDto> UpdatePrescriptionAsync(UpdatePrescriptionDto dto)
        {
            var prescription = await _context.Prescriptions.FindAsync(dto.Id);
            if (prescription == null)
                throw new KeyNotFoundException($"Prescription with ID {dto.Id} not found");

            prescription.Remark = dto.Remark;
            prescription.isDispensed = dto.isDispensed;

            _context.Prescriptions.Update(prescription);
            await _context.SaveChangesAsync();

            return await GetPrescriptionByIdAsync(dto.Id);
        }

        // DELETE
        public async Task<bool> DeletePrescriptionAsync(Guid id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionItems)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (prescription == null)
                throw new KeyNotFoundException($"Prescription with ID {id} not found");

            // Delete prescription items first
            _context.PrescriptionItems.RemoveRange(prescription.PrescriptionItems);

            // Then delete prescription
            _context.Prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();

            return true;
        }

        // Helper method to map to DTO
        private async Task<PrescriptionResponseDto> MapToResponseDtoAsync(Prescription prescription)
        {
            return new PrescriptionResponseDto
            {
                Id = prescription.Id,
                AppointmentId = prescription.AppointmentId,
                DatePrescribed = prescription.DatePrescribed,
                Remark = prescription.Remark,
                isDispensed = prescription.isDispensed,
                PrescriptionItems = prescription.PrescriptionItems.Select(pi => new PrescriptionItemResponseDto
                {
                    Id = pi.Id,
                    MedicineId = pi.MedicineId,
                    MedicineName = pi.Medicine?.Name ?? "Unknown",
                    Quantity = pi.Quantity,
                    Dosage = pi.Dosage,
                    DurationDays = pi.DurationDays
                }).ToList()
            };
        }
    }
}
