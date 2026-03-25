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
    [Authorize(Roles = "Admin,Doctor")]
    public class PrescriptionsController : ControllerBase
    {
        private readonly PrescriptionService _prescriptionService;

        public PrescriptionsController(PrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        /// <summary>
        /// Create a new prescription - Doctor only
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<PrescriptionResponseDto>> CreatePrescription([FromBody] CreatePrescriptionDto dto)
        {
            try
            {
                var result = await _prescriptionService.CreatePrescriptionAsync(dto);
                return CreatedAtAction(nameof(GetPrescriptionById), new { id = result.Id }, result);
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
        /// Get all prescriptions - Admin only
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<PrescriptionResponseDto>>> GetAllPrescriptions()
        {
            try
            {
                var result = await _prescriptionService.GetAllPrescriptionsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get prescription by ID - Admin and Doctor
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionResponseDto>> GetPrescriptionById(Guid id)
        {
            try
            {
                var result = await _prescriptionService.GetPrescriptionByIdAsync(id);
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
        /// Get prescriptions by Appointment ID - Admin and Doctor
        /// </summary>
        [HttpGet("appointment/{appointmentId}")]
        public async Task<ActionResult<List<PrescriptionResponseDto>>> GetPrescriptionsByAppointmentId(Guid appointmentId)
        {
            try
            {
                var result = await _prescriptionService.GetPrescriptionsByAppointmentIdAsync(appointmentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update prescription - Doctor and Admin
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PrescriptionResponseDto>> UpdatePrescription(Guid id, [FromBody] UpdatePrescriptionDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var result = await _prescriptionService.UpdatePrescriptionAsync(dto);
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
        /// Delete prescription - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeletePrescription(Guid id)
        {
            try
            {
                var result = await _prescriptionService.DeletePrescriptionAsync(id);
                return Ok(new { message = "Prescription deleted successfully" });
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
