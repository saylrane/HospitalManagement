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
    public class BillService
    {
        private readonly ApplicationDBContext _context;

        public BillService(ApplicationDBContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<BillResponseDto> CreateBillAsync(CreateBillDto dto)
        {
            var patient = await _context.Patients.FindAsync(dto.PatientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient with ID {dto.PatientId} not found");

            var bill = new Bill
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                Amount = dto.Amount,
                IsPaid = false,
                CreatedAt = DateTime.Now
            };

            _context.Bill.Add(bill);
            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(bill);
        }

        // READ ALL
        public async Task<List<BillResponseDto>> GetAllBillsAsync()
        {
            var bills = await _context.Bill
                .Include(b => b.Patient)
                .ToListAsync();

            var dtos = new List<BillResponseDto>();
            foreach (var bill in bills)
            {
                dtos.Add(await MapToResponseDtoAsync(bill));
            }
            return dtos;
        }

        // READ BY ID
        public async Task<BillResponseDto> GetBillByIdAsync(Guid id)
        {
            var bill = await _context.Bill
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bill == null)
                throw new KeyNotFoundException($"Bill with ID {id} not found");

            return await MapToResponseDtoAsync(bill);
        }

        // READ BY PATIENT ID
        public async Task<List<BillResponseDto>> GetBillsByPatientIdAsync(Guid patientId)
        {
            var bills = await _context.Bill
                .Where(b => b.PatientId == patientId)
                .Include(b => b.Patient)
                .ToListAsync();

            var dtos = new List<BillResponseDto>();
            foreach (var bill in bills)
            {
                dtos.Add(await MapToResponseDtoAsync(bill));
            }
            return dtos;
        }

        // READ UNPAID BILLS
        public async Task<List<BillResponseDto>> GetUnpaidBillsAsync()
        {
            var bills = await _context.Bill
                .Where(b => !b.IsPaid)
                .Include(b => b.Patient)
                .ToListAsync();

            var dtos = new List<BillResponseDto>();
            foreach (var bill in bills)
            {
                dtos.Add(await MapToResponseDtoAsync(bill));
            }
            return dtos;
        }

        // UPDATE
        public async Task<BillResponseDto> UpdateBillAsync(UpdateBillDto dto)
        {
            var bill = await _context.Bill.FindAsync(dto.Id);
            if (bill == null)
                throw new KeyNotFoundException($"Bill with ID {dto.Id} not found");

            bill.Amount = dto.Amount;
            bill.IsPaid = dto.IsPaid;

            _context.Bill.Update(bill);
            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(bill);
        }

        // DELETE
        public async Task<bool> DeleteBillAsync(Guid id)
        {
            var bill = await _context.Bill.FindAsync(id);
            if (bill == null)
                throw new KeyNotFoundException($"Bill with ID {id} not found");

            _context.Bill.Remove(bill);
            await _context.SaveChangesAsync();

            return true;
        }

        // Helper method to map to DTO
        private async Task<BillResponseDto> MapToResponseDtoAsync(Bill bill)
        {
            return new BillResponseDto
            {
                Id = bill.Id,
                PatientId = bill.PatientId,
                PatientName = bill.Patient?.PatientName ?? "Unknown",
                CreatedAt = bill.CreatedAt,
                Amount = bill.Amount,
                IsPaid = bill.IsPaid
            };
        }
    }
}
