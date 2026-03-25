using HospitalManagement.Application.DTOs;
using HospitalManagement.Domain.Models;
using HospitalManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagement.Application.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDBContext _context;

        public AppointmentService(ApplicationDBContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto dto)
        {
            var patient = await _context.Patients.FindAsync(dto.PatientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient with ID {dto.PatientId} not found");

            if (patient.IsDeleted)
                throw new InvalidOperationException("Cannot book appointment for deleted patient");

            // Check if patient already has appointment at same time
            var patientConflict = await _context.Appointment
                .Where(a => a.PatientId == dto.PatientId
                    && a.AppointmentDate == dto.AppointmentDate
                    && a.AppointmentStatus != "Cancelled"
                    && ((a.AppointmentStartTime < dto.AppointmentEndTime && a.AppointmentEndTime > dto.AppointmentStartTime)))
                .FirstOrDefaultAsync();

            if (patientConflict != null)
                throw new InvalidOperationException("Patient already has an appointment at this time");

            if (dto.DoctorId.HasValue)
            {
                var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
                if (doctor == null)
                    throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} not found");

                if (doctor.IsDeleted)
                    throw new InvalidOperationException("Cannot book appointment with deleted doctor");

                // Check if doctor is available at this time
                var doctorConflict = await _context.Appointment
                    .Where(a => a.DoctorId == dto.DoctorId
                        && a.AppointmentDate == dto.AppointmentDate
                        && a.AppointmentStatus != "Cancelled"
                        && ((a.AppointmentStartTime < dto.AppointmentEndTime && a.AppointmentEndTime > dto.AppointmentStartTime)))
                    .FirstOrDefaultAsync();

                if (doctorConflict != null)
                    throw new InvalidOperationException("Doctor is not available at this time");
            }

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                AppointmentStartTime = dto.AppointmentStartTime,
                AppointmentEndTime = dto.AppointmentEndTime,
                AppointmentStatus = "Booked",
                CreatedAt = DateTime.Now
            };

            _context.Appointment.Add(appointment);
            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(appointment);
        }

        // READ ALL
        public async Task<List<AppointmentResponseDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _context.Appointment
                .Where(a => a.AppointmentStatus != "Cancelled")
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();

            var dtos = new List<AppointmentResponseDto>();
            foreach (var appointment in appointments)
            {
                dtos.Add(await MapToResponseDtoAsync(appointment));
            }
            return dtos;
        }

        // READ BY ID
        public async Task<AppointmentResponseDto> GetAppointmentByIdAsync(Guid id)
        {
            var appointment = await _context.Appointment
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found");

            return await MapToResponseDtoAsync(appointment);
        }

        // READ APPOINTMENTS BY PATIENT
        public async Task<List<AppointmentResponseDto>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            var appointments = await _context.Appointment
                .Where(a => a.PatientId == patientId && a.AppointmentStatus != "Cancelled")
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();

            var dtos = new List<AppointmentResponseDto>();
            foreach (var appointment in appointments)
            {
                dtos.Add(await MapToResponseDtoAsync(appointment));
            }
            return dtos;
        }

        // READ APPOINTMENTS BY DOCTOR
        public async Task<List<AppointmentResponseDto>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            var appointments = await _context.Appointment
                .Where(a => a.DoctorId == doctorId && a.AppointmentStatus != "Cancelled")
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();

            var dtos = new List<AppointmentResponseDto>();
            foreach (var appointment in appointments)
            {
                dtos.Add(await MapToResponseDtoAsync(appointment));
            }
            return dtos;
        }

        // UPDATE
        public async Task<AppointmentResponseDto> UpdateAppointmentAsync(UpdateAppointmentDto dto)
        {
            var appointment = await _context.Appointment.FindAsync(dto.Id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {dto.Id} not found");

            var patient = await _context.Patients.FindAsync(dto.PatientId);
            if (patient == null)
                throw new KeyNotFoundException($"Patient with ID {dto.PatientId} not found");

            if (dto.DoctorId.HasValue)
            {
                var doctor = await _context.Doctors.FindAsync(dto.DoctorId);
                if (doctor == null)
                    throw new KeyNotFoundException($"Doctor with ID {dto.DoctorId} not found");
            }

            appointment.PatientId = dto.PatientId;
            appointment.DoctorId = dto.DoctorId;
            appointment.AppointmentDate = dto.AppointmentDate;
            appointment.AppointmentStartTime = dto.AppointmentStartTime;
            appointment.AppointmentEndTime = dto.AppointmentEndTime;
            appointment.AppointmentStatus = dto.AppointmentStatus;

            _context.Appointment.Update(appointment);
            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(appointment);
        }

        // UPDATE STATUS
        public async Task<AppointmentResponseDto> UpdateAppointmentStatusAsync(UpdateAppointmentStatusDto dto)
        {
            var appointment = await _context.Appointment.FindAsync(dto.Id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {dto.Id} not found");

            appointment.AppointmentStatus = dto.AppointmentStatus;

            _context.Appointment.Update(appointment);
            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(appointment);
        }

        // DELETE
        public async Task<bool> DeleteAppointmentAsync(Guid id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found");

            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();

            return true;
        }

        // CANCEL APPOINTMENT (Set status to cancelled - frees up slot)
        public async Task<AppointmentResponseDto> CancelAppointmentAsync(Guid id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException($"Appointment with ID {id} not found");

            if (appointment.AppointmentStatus == "Cancelled")
                throw new InvalidOperationException("Appointment is already cancelled");

            appointment.AppointmentStatus = "Cancelled";

            _context.Appointment.Update(appointment);
            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(appointment);
        }

        // Helper method to map to DTO
        private async Task<AppointmentResponseDto> MapToResponseDtoAsync(Appointment appointment)
        {
            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient?.PatientName ?? "Unknown",
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.Doctor?.Name ?? "Not Assigned",
                AppointmentDate = appointment.AppointmentDate,
                AppointmentStartTime = appointment.AppointmentStartTime,
                AppointmentEndTime = appointment.AppointmentEndTime,
                CreatedAt = appointment.CreatedAt,
                AppointmentStatus = appointment.AppointmentStatus
            };
        }
    }
}
