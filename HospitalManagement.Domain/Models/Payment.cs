using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class Payment
    {
        [Key]

        public Guid Id { get; set; }

        [ForeignKey("Bill")]
        public Guid BillId { get; set; }

        public Bill? Bill { get; set; }


        public DateTime PaidAt { get; set; }

        public decimal Amount { get; set; }

        public string? PaymentMethod { get; set; }

    }
}
