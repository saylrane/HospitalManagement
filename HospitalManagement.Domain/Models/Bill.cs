using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class Bill
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Patient")]
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }
         
        public DateTime CreatedAt { get; set; }

        public decimal Amount { get; set; }

        public bool IsPaid { get; set; }


        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}
