using System;

namespace HospitalManagement.Application.DTOs
{
    public class CreateBillDto
    {
        public Guid PatientId { get; set; }
        public decimal Amount { get; set; }
    }

    public class UpdateBillDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }

    public class BillResponseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public bool IsPaid { get; set; }
    }
}
