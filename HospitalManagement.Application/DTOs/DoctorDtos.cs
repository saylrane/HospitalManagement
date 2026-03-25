using System;

namespace HospitalManagement.Application.DTOs
{
    public class CreateDoctorDto
    {
        public string Name { get; set; }
        public string Specialty { get; set; }
    }

    public class UpdateDoctorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
    }

    public class DoctorResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
