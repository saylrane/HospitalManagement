using System;
using System.Collections.Generic;

namespace HospitalManagement.Application.DTOs
{
    public class CreatePrescriptionDto
    {
        public Guid AppointmentId { get; set; }
        public string Remark { get; set; }
        public List<CreatePrescriptionItemDto> PrescriptionItems { get; set; }
    }

    public class CreatePrescriptionItemDto
    {
        public Guid MedicineId { get; set; }
        public int Quantity { get; set; }
        public string Dosage { get; set; }
        public int DurationDays { get; set; }
    }

    public class UpdatePrescriptionDto
    {
        public Guid Id { get; set; }
        public string Remark { get; set; }
        public bool isDispensed { get; set; }
    }

    public class PrescriptionResponseDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public DateTime DatePrescribed { get; set; }
        public string Remark { get; set; }
        public bool isDispensed { get; set; }
        public List<PrescriptionItemResponseDto> PrescriptionItems { get; set; }
    }

    public class PrescriptionItemResponseDto
    {
        public Guid Id { get; set; }
        public Guid MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public string Dosage { get; set; }
        public int DurationDays { get; set; }
    }
}
