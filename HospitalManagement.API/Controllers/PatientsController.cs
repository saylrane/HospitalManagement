using HospitalManagement.Application.DTOs;
using HospitalManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HospitalManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientsController(PatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>
        /// Create a new patient - Patients can register themselves, Admin can create any patient
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<PatientResponseDto>> CreatePatient([FromBody] CreatePatientDto dto)
        {
            try
            {
                var result = await _patientService.CreatePatientAsync(dto);
                return CreatedAtAction(nameof(GetPatientById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all patients - Admin only
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PatientResponseDto>>> GetAllPatients()
        {
            try
            {
                var result = await _patientService.GetAllPatientsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get patient by ID - Admin, Doctor, and the patient themselves
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<PatientResponseDto>> GetPatientById(Guid id)
        {
            try
            {
                var result = await _patientService.GetPatientByIdAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update patient - Admin only
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PatientResponseDto>> UpdatePatient(Guid id, [FromBody] UpdatePatientDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var result = await _patientService.UpdatePatientAsync(dto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete patient - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeletePatient(Guid id)
        {
            try
            {
                var result = await _patientService.DeletePatientAsync(id);
                return Ok(new { message = "Patient deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
