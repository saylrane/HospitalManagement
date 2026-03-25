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
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentsController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Create a new appointment - Patients can book their own, Admin and Receptionist can create any
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.CreateAppointmentAsync(dto);
                return CreatedAtAction(nameof(GetAppointmentById), new { id = result.Id }, result);
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
        /// Get all appointments - Admin and Receptionist
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<ActionResult<List<AppointmentResponseDto>>> GetAllAppointments()
        {
            try
            {
                var result = await _appointmentService.GetAllAppointmentsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get appointment by ID - All authenticated users
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentResponseDto>> GetAppointmentById(Guid id)
        {
            try
            {
                var result = await _appointmentService.GetAppointmentByIdAsync(id);
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
        /// Get appointments by Patient ID - Patient, Doctor, Admin
        /// </summary>
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<List<AppointmentResponseDto>>> GetAppointmentsByPatientId(Guid patientId)
        {
            try
            {
                var result = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get appointments by Doctor ID - Doctor, Admin
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Admin,Doctor")]
        public async Task<ActionResult<List<AppointmentResponseDto>>> GetAppointmentsByDoctorId(Guid doctorId)
        {
            try
            {
                var result = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update appointment - Admin and Receptionist
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var result = await _appointmentService.UpdateAppointmentAsync(dto);
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
        /// Update appointment status - Admin, Doctor, and Receptionist
        /// </summary>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Doctor,Receptionist")]
        public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointmentStatus(Guid id, [FromBody] UpdateAppointmentStatusDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var result = await _appointmentService.UpdateAppointmentStatusAsync(dto);
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
        /// Delete appointment - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteAppointment(Guid id)
        {
            try
            {
                var result = await _appointmentService.DeleteAppointmentAsync(id);
                return Ok(new { message = "Appointment deleted successfully" });
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
        /// Cancel appointment - Patient, Admin, Receptionist (Frees up the slot for other patients)
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = "Admin,Receptionist,Patient")]
        public async Task<ActionResult<AppointmentResponseDto>> CancelAppointment(Guid id)
        {
            try
            {
                var result = await _appointmentService.CancelAppointmentAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
