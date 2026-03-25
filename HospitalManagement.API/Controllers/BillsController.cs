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
    [Authorize(Roles = "Admin,Receptionist")]
    public class BillsController : ControllerBase
    {
        private readonly BillService _billService;

        public BillsController(BillService billService)
        {
            _billService = billService;
        }

        /// <summary>
        /// Create a new bill - Receptionist and Admin
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BillResponseDto>> CreateBill([FromBody] CreateBillDto dto)
        {
            try
            {
                var result = await _billService.CreateBillAsync(dto);
                return CreatedAtAction(nameof(GetBillById), new { id = result.Id }, result);
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
        /// Get all bills - Receptionist and Admin
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<BillResponseDto>>> GetAllBills()
        {
            try
            {
                var result = await _billService.GetAllBillsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get bill by ID - Patient, Receptionist, Admin
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<BillResponseDto>> GetBillById(Guid id)
        {
            try
            {
                var result = await _billService.GetBillByIdAsync(id);
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
        /// Get bills by Patient ID - Patient, Receptionist, Admin
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<BillResponseDto>>> GetBillsByPatientId(Guid patientId)
        {
            try
            {
                var result = await _billService.GetBillsByPatientIdAsync(patientId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get unpaid bills - Receptionist and Admin
        /// </summary>
        [HttpGet("status/unpaid")]
        public async Task<ActionResult<List<BillResponseDto>>> GetUnpaidBills()
        {
            try
            {
                var result = await _billService.GetUnpaidBillsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update bill - Receptionist and Admin
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<BillResponseDto>> UpdateBill(Guid id, [FromBody] UpdateBillDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var result = await _billService.UpdateBillAsync(dto);
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
        /// Delete bill - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteBill(Guid id)
        {
            try
            {
                var result = await _billService.DeleteBillAsync(id);
                return Ok(new { message = "Bill deleted successfully" });
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
