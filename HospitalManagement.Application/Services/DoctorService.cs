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
    public class DoctorService
    {
        private readonly ApplicationDBContext _context;

        public DoctorService(ApplicationDBContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<DoctorResponseDto> CreateDoctorAsync(CreateDoctorDto dto)
        {
            var doctor = new Doctor
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Specialty = dto.Specialty,
                CreatedAt = DateTime.Now
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return MapToResponseDto(doctor);
        }

        // READ ALL
        public async Task<List<DoctorResponseDto>> GetAllDoctorsAsync()
        {
            var doctors = await _context.Doctors
                .Where(d => !d.IsDeleted)
                .ToListAsync();
            return doctors.Select(MapToResponseDto).ToList();
        }

        // READ BY ID
        public async Task<DoctorResponseDto> GetDoctorByIdAsync(Guid id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {id} not found");

            return MapToResponseDto(doctor);
        }

        // UPDATE
        public async Task<DoctorResponseDto> UpdateDoctorAsync(UpdateDoctorDto dto)
        {
            var doctor = await _context.Doctors.FindAsync(dto.Id);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {dto.Id} not found");

            doctor.Name = dto.Name;
            doctor.Specialty = dto.Specialty;

            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();

            return MapToResponseDto(doctor);
        }

        // DELETE
        public async Task<bool> DeleteDoctorAsync(Guid id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
                throw new KeyNotFoundException($"Doctor with ID {id} not found");

            doctor.IsDeleted = true;

            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();

            return true;
        }

        // Helper method to map to DTO
        private DoctorResponseDto MapToResponseDto(Doctor doctor)
        {
            return new DoctorResponseDto
            {
                Id = doctor.Id,
                Name = doctor.Name,
                Specialty = doctor.Specialty,
                CreatedAt = doctor.CreatedAt
            };
        }
    }
}
