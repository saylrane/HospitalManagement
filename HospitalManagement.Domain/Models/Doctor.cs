using System;
using System.Collections.Generic;
using System.Text;

namespace HospitalManagement.Domain.Models
{
    public class Doctor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
