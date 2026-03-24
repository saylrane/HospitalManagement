using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class Patient
    {
        [Key]
        public Guid Id { get; set; }
        public string PatientName { get; set; }
        public string PatientContact { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Allergies { get; set; }
        public string PatientAddress { get; set; }
        public int Age { get; set; }
        public string Bloodgroup { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public ICollection<Bill> Bills { get; set; } = new List<Bill>();

        //public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();


    }
}
