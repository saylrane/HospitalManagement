using System;

namespace HospitalManagement.Application.DTOs
{
    public class CreateInventoryDto
    {
        public Guid MedicineId { get; set; }
        public int StockQuantity { get; set; }
    }

    public class UpdateInventoryDto
    {
        public Guid Id { get; set; }
        public int StockQuantity { get; set; }
    }

    public class InventoryResponseDto
    {
        public Guid Id { get; set; }
        public Guid MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int StockQuantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
