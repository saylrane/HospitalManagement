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
    public class PatientService
    {
        private readonly ApplicationDBContext _context;

        public PatientService(ApplicationDBContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<PatientResponseDto> CreatePatientAsync(CreatePatientDto dto)
        {
            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                PatientName = dto.PatientName,
                PatientContact = dto.PatientContact,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Allergies = dto.Allergies,
                PatientAddress = dto.PatientAddress,
                Bloodgroup = dto.Bloodgroup,
                Age = CalculateAge(dto.DateOfBirth),
                CreatedAt = DateTime.Now
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return MapToResponseDto(patient);
        }

        // READ ALL
        public async Task<List<PatientResponseDto>> GetAllPatientsAsync()
        {
            var patients = await _context.Patients
                .Where(p => !p.IsDeleted)
                .ToListAsync();
            return patients.Select(MapToResponseDto).ToList();
        }

        // READ BY ID
        public async Task<PatientResponseDto> GetPatientByIdAsync(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                throw new KeyNotFoundException($"Patient with ID {id} not found");

            return MapToResponseDto(patient);
        }

        // UPDATE
        public async Task<PatientResponseDto> UpdatePatientAsync(UpdatePatientDto dto)
        {
            var patient = await _context.Patients.FindAsync(dto.Id);
            if (patient == null)
                throw new KeyNotFoundException($"Patient with ID {dto.Id} not found");

            patient.PatientName = dto.PatientName;
            patient.PatientContact = dto.PatientContact;
            patient.DateOfBirth = dto.DateOfBirth;
            patient.Gender = dto.Gender;
            patient.Allergies = dto.Allergies;
            patient.PatientAddress = dto.PatientAddress;
            patient.Bloodgroup = dto.Bloodgroup;
            patient.Age = CalculateAge(dto.DateOfBirth);

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();

            return MapToResponseDto(patient);
        }

        // DELETE (Soft Delete)
        public async Task<bool> DeletePatientAsync(Guid id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
                throw new KeyNotFoundException($"Patient with ID {id} not found");

            patient.IsDeleted = true;

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();

            return true;
        }

        // Helper method to calculate age
        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }

        // Helper method to map to DTO
        private PatientResponseDto MapToResponseDto(Patient patient)
        {
            return new PatientResponseDto
            {
                Id = patient.Id,
                PatientName = patient.PatientName,
                PatientContact = patient.PatientContact,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Allergies = patient.Allergies,
                PatientAddress = patient.PatientAddress,
                Age = patient.Age,
                Bloodgroup = patient.Bloodgroup,
                CreatedAt = patient.CreatedAt
            };
        }
    }
}
