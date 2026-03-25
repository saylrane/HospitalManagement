using System;

namespace HospitalManagement.Application.DTOs
{
    public class CreateAppointmentDto
    {
        public Guid PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeOnly AppointmentStartTime { get; set; }
        public TimeOnly AppointmentEndTime { get; set; }
    }

    public class UpdateAppointmentDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeOnly AppointmentStartTime { get; set; }
        public TimeOnly AppointmentEndTime { get; set; }
        public string AppointmentStatus { get; set; }
    }

    public class UpdateAppointmentStatusDto
    {
        public Guid Id { get; set; }
        public string AppointmentStatus { get; set; }
    }

    public class AppointmentResponseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; }
        public Guid? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeOnly AppointmentStartTime { get; set; }
        public TimeOnly AppointmentEndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AppointmentStatus { get; set; }
    }
}
