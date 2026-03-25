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
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// Create a new payment - Receptionist and Admin
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            try
            {
                var result = await _paymentService.CreatePaymentAsync(dto);
                return CreatedAtAction(nameof(GetPaymentById), new { id = result.Id }, result);
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

        /// <summary>
        /// Get all payments - Receptionist and Admin
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PaymentResponseDto>>> GetAllPayments()
        {
            try
            {
                var result = await _paymentService.GetAllPaymentsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get payment by ID - Receptionist and Admin
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentResponseDto>> GetPaymentById(Guid id)
        {
            try
            {
                var result = await _paymentService.GetPaymentByIdAsync(id);
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
        /// Get payments by Bill ID - Receptionist and Admin
        /// </summary>
        [HttpGet("bill/{billId}")]
        public async Task<ActionResult<List<PaymentResponseDto>>> GetPaymentsByBillId(Guid billId)
        {
            try
            {
                var result = await _paymentService.GetPaymentsByBillIdAsync(billId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update payment - Receptionist and Admin
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PaymentResponseDto>> UpdatePayment(Guid id, [FromBody] UpdatePaymentDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var result = await _paymentService.UpdatePaymentAsync(dto);
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
        /// Delete payment - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeletePayment(Guid id)
        {
            try
            {
                var result = await _paymentService.DeletePaymentAsync(id);
                return Ok(new { message = "Payment deleted successfully" });
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
