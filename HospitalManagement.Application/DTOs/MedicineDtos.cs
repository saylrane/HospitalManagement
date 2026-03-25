using System;

namespace HospitalManagement.Application.DTOs
{
    public class CreateMedicineDto
    {
        public string Name { get; set; }
        public string ManufacturerName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class UpdateMedicineDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ManufacturerName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }

    public class MedicineResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ManufacturerName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
