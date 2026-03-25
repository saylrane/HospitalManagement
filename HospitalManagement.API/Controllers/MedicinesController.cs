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
    [Authorize(Roles = "Admin,Pharmacist")]
    public class MedicinesController : ControllerBase
    {
        private readonly MedicineService _medicineService;

        public MedicinesController(MedicineService medicineService)
        {
            _medicineService = medicineService;
        }

        /// <summary>
        /// Create a new medicine - Pharmacist and Admin
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<MedicineResponseDto>> CreateMedicine([FromBody] CreateMedicineDto dto)
        {
            try
            {
                var result = await _medicineService.CreateMedicineAsync(dto);
                return CreatedAtAction(nameof(GetMedicineById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all medicines - All authenticated users
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<MedicineResponseDto>>> GetAllMedicines()
        {
            try
            {
                var result = await _medicineService.GetAllMedicinesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get medicine by ID - All users
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<MedicineResponseDto>> GetMedicineById(Guid id)
        {
            try
            {
                var result = await _medicineService.GetMedicineByIdAsync(id);
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
        /// Update medicine - Pharmacist and Admin
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<MedicineResponseDto>> UpdateMedicine(Guid id, [FromBody] UpdateMedicineDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var result = await _medicineService.UpdateMedicineAsync(dto);
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
        /// Delete medicine - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteMedicine(Guid id)
        {
            try
            {
                var result = await _medicineService.DeleteMedicineAsync(id);
                return Ok(new { message = "Medicine deleted successfully" });
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
        /// Update medicine stock - Pharmacist and Admin
        /// </summary>
        [HttpPatch("{id}/stock")]
        public async Task<ActionResult<MedicineResponseDto>> UpdateMedicineStock(Guid id, [FromBody] int quantity)
        {
            try
            {
                var result = await _medicineService.UpdateMedicineStockAsync(id, quantity);
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
    }
}
