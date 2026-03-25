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
    public class InventoryService
    {
        private readonly ApplicationDBContext _context;

        public InventoryService(ApplicationDBContext context)
        {
            _context = context;
        }

        // CREATE - Log inventory transaction
        public async Task<InventoryResponseDto> CreateInventoryAsync(CreateInventoryDto dto)
        {
            var medicine = await _context.Medicines.FindAsync(dto.MedicineId);
            if (medicine == null)
                throw new KeyNotFoundException($"Medicine with ID {dto.MedicineId} not found");

            var inventoryLog = new InventoryLogs
            {
                Id = Guid.NewGuid(),
                MedicineId = dto.MedicineId,
                QuantityChanged = dto.StockQuantity,
                ActionType = "Manual update",
                Remarks = "Initial stock entry"
            };

            _context.InventoryLogs.Add(inventoryLog);

            // Update medicine stock
            medicine.StockQuantity = dto.StockQuantity;
            medicine.LastUpdated = DateTime.Now;
            _context.Medicines.Update(medicine);

            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(inventoryLog);
        }

        // READ ALL
        public async Task<List<InventoryResponseDto>> GetAllInventoriesAsync()
        {
            var inventories = await _context.InventoryLogs
                .Include(i => i.Medicine)
                .ToListAsync();

            var dtos = new List<InventoryResponseDto>();
            foreach (var inventory in inventories)
            {
                dtos.Add(await MapToResponseDtoAsync(inventory));
            }
            return dtos;
        }

        // READ BY ID
        public async Task<InventoryResponseDto> GetInventoryByIdAsync(Guid id)
        {
            var inventory = await _context.InventoryLogs
                .Include(i => i.Medicine)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
                throw new KeyNotFoundException($"Inventory with ID {id} not found");

            return await MapToResponseDtoAsync(inventory);
        }

        // READ BY MEDICINE ID
        public async Task<List<InventoryResponseDto>> GetInventoriesByMedicineIdAsync(Guid medicineId)
        {
            var inventories = await _context.InventoryLogs
                .Where(i => i.MedicineId == medicineId)
                .Include(i => i.Medicine)
                .ToListAsync();

            var dtos = new List<InventoryResponseDto>();
            foreach (var inventory in inventories)
            {
                dtos.Add(await MapToResponseDtoAsync(inventory));
            }
            return dtos;
        }

        // UPDATE - Log inventory update
        public async Task<InventoryResponseDto> UpdateInventoryAsync(UpdateInventoryDto dto)
        {
            var inventoryLog = await _context.InventoryLogs.FindAsync(dto.Id);
            if (inventoryLog == null)
                throw new KeyNotFoundException($"Inventory log with ID {dto.Id} not found");

            var medicine = await _context.Medicines.FindAsync(inventoryLog.MedicineId);
            if (medicine == null)
                throw new KeyNotFoundException($"Medicine with ID {inventoryLog.MedicineId} not found");

            // Calculate the difference
            int quantityDifference = dto.StockQuantity - medicine.StockQuantity;

            // Create new log entry for the update
            var newLog = new InventoryLogs
            {
                Id = Guid.NewGuid(),
                MedicineId = inventoryLog.MedicineId,
                QuantityChanged = quantityDifference,
                ActionType = "Manual update",
                Remarks = $"Stock updated from {medicine.StockQuantity} to {dto.StockQuantity}"
            };

            _context.InventoryLogs.Add(newLog);

            // Update medicine stock
            medicine.StockQuantity = dto.StockQuantity;
            medicine.LastUpdated = DateTime.Now;
            _context.Medicines.Update(medicine);

            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(newLog);
        }

        // DELETE
        public async Task<bool> DeleteInventoryAsync(Guid id)
        {
            var inventory = await _context.InventoryLogs.FindAsync(id);
            if (inventory == null)
                throw new KeyNotFoundException($"Inventory with ID {id} not found");

            _context.InventoryLogs.Remove(inventory);
            await _context.SaveChangesAsync();

            return true;
        }

        // Helper method to map to DTO
        private async Task<InventoryResponseDto> MapToResponseDtoAsync(InventoryLogs inventory)
        {
            return new InventoryResponseDto
            {
                Id = inventory.Id,
                MedicineId = inventory.MedicineId,
                MedicineName = inventory.Medicine?.Name ?? "Unknown",
                StockQuantity = inventory.QuantityChanged,
                LastUpdated = DateTime.Now
            };
        }
    }
}

