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
    public class DoctorsController : ControllerBase
    {
        private readonly DoctorService _doctorService;

        public DoctorsController(DoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        /// <summary>
        /// Create a new doctor - Doctors can register themselves, Admin can create any doctor
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<DoctorResponseDto>> CreateDoctor([FromBody] CreateDoctorDto dto)
        {
            try
            {
                var result = await _doctorService.CreateDoctorAsync(dto);
                return CreatedAtAction(nameof(GetDoctorById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all doctors - All authenticated users
        /// </summary>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<DoctorResponseDto>>> GetAllDoctors()
        {
            try
            {
                var result = await _doctorService.GetAllDoctorsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get doctor by ID - All authenticated users
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<DoctorResponseDto>> GetDoctorById(Guid id)
        {
            try
            {
                var result = await _doctorService.GetDoctorByIdAsync(id);
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
        /// Update doctor - Admin only
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DoctorResponseDto>> UpdateDoctor(Guid id, [FromBody] UpdateDoctorDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var result = await _doctorService.UpdateDoctorAsync(dto);
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
        /// Delete doctor - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteDoctor(Guid id)
        {
            try
            {
                var result = await _doctorService.DeleteDoctorAsync(id);
                return Ok(new { message = "Doctor deleted successfully" });
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
