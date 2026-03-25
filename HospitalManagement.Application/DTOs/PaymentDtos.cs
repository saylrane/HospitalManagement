using System;

namespace HospitalManagement.Application.DTOs
{
    public class CreatePaymentDto
    {
        public Guid BillId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class UpdatePaymentDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class PaymentResponseDto
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public DateTime PaidAt { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }
}
