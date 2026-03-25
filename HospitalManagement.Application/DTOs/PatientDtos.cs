using System;

namespace HospitalManagement.Application.DTOs
{
    public class CreatePatientDto
    {
        public string PatientName { get; set; }
        public string PatientContact { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Allergies { get; set; }
        public string PatientAddress { get; set; }
        public string Bloodgroup { get; set; }
    }

    public class UpdatePatientDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; }
        public string PatientContact { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Allergies { get; set; }
        public string PatientAddress { get; set; }
        public string Bloodgroup { get; set; }
    }

    public class PatientResponseDto
    {
        public Guid Id { get; set; }
        public string PatientName { get; set; }
        public string PatientContact { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Allergies { get; set; }
        public string PatientAddress { get; set; }
        public int Age { get; set; }
        public string Bloodgroup { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
