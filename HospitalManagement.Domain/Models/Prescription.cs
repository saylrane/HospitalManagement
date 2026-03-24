using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class Prescription
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Appointment")]
        public Guid AppointmentId { get; set; }
        public Appointment? Appointment { get; set; }

        //[ForeignKey("Patient")]
        //public Patient Patient { get; set; }
        //public Guid PatientId { get; set; }

        //[ForeignKey("Medicine")]
        //public Medicine Medicine { get; set; }
        //public Guid MedicineId { get; set; }
        //public int Quantity { get; set; }
        public DateTime DatePrescribed { get; set; } = DateTime.Now;

        public string Remark { get; set; }

        //public int CourseDurationDays { get; set; }
           
        public ICollection<PrescriptionItems> PrescriptionItems { get; set; } = new List<PrescriptionItems>();

    }
}
