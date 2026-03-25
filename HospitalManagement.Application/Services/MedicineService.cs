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
    public class MedicineService
    {
        private readonly ApplicationDBContext _context;

        public MedicineService(ApplicationDBContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<MedicineResponseDto> CreateMedicineAsync(CreateMedicineDto dto)
        {
            var medicine = new Medicine
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ManufacturerName = dto.ManufacturerName,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                LastUpdated = DateTime.Now
            };

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            return MapToResponseDto(medicine);
        }

        // READ ALL
        public async Task<List<MedicineResponseDto>> GetAllMedicinesAsync()
        {
            var medicines = await _context.Medicines.ToListAsync();
            return medicines.Select(MapToResponseDto).ToList();
        }

        // READ BY ID
        public async Task<MedicineResponseDto> GetMedicineByIdAsync(Guid id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
                throw new KeyNotFoundException($"Medicine with ID {id} not found");

            return MapToResponseDto(medicine);
        }

        // UPDATE
        public async Task<MedicineResponseDto> UpdateMedicineAsync(UpdateMedicineDto dto)
        {
            var medicine = await _context.Medicines.FindAsync(dto.Id);
            if (medicine == null)
                throw new KeyNotFoundException($"Medicine with ID {dto.Id} not found");

            medicine.Name = dto.Name;
            medicine.ManufacturerName = dto.ManufacturerName;
            medicine.Price = dto.Price;
            medicine.StockQuantity = dto.StockQuantity;
            medicine.LastUpdated = DateTime.Now;

            _context.Medicines.Update(medicine);
            await _context.SaveChangesAsync();

            return MapToResponseDto(medicine);
        }

        // DELETE
        public async Task<bool> DeleteMedicineAsync(Guid id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
                throw new KeyNotFoundException($"Medicine with ID {id} not found");

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();

            return true;
        }

        // UPDATE STOCK
        public async Task<MedicineResponseDto> UpdateMedicineStockAsync(Guid id, int quantity)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
                throw new KeyNotFoundException($"Medicine with ID {id} not found");

            medicine.StockQuantity = quantity;
            medicine.LastUpdated = DateTime.Now;

            _context.Medicines.Update(medicine);
            await _context.SaveChangesAsync();

            return MapToResponseDto(medicine);
        }

        // Helper method to map to DTO
        private MedicineResponseDto MapToResponseDto(Medicine medicine)
        {
            return new MedicineResponseDto
            {
                Id = medicine.Id,
                Name = medicine.Name,
                ManufacturerName = medicine.ManufacturerName,
                Price = medicine.Price,
                StockQuantity = medicine.StockQuantity,
                LastUpdated = medicine.LastUpdated
            };
        }
    }
}
