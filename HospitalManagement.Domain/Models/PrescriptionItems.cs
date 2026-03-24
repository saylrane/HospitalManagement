using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class PrescriptionItems
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("PrescriptionId")]
        public Guid PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }

        [ForeignKey("MedicineId")]
        public Guid MedicineId { get; set; }
        public Medicine Medicine { get; set; }

        public int Quantity { get; set; }
        public string Dosage { get; set; }    // "500 mg"
        public int DurationDays { get; set; } // number of days
    }
}
