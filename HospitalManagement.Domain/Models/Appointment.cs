using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class Appointment
    {
        [Key]
        public Guid Id { get; set; }

        [ForeignKey("Patient")]
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; }

        [ForeignKey("Doctor")]
        public Guid? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }
        

        public DateTime AppointmentDate { get; set; }
        public TimeOnly AppointmentStartTime { get; set; }
        public TimeOnly AppointmentEndTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string AppointmentStatus { get; set; } //Booked, Scheduled, completed, cancelled

        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
