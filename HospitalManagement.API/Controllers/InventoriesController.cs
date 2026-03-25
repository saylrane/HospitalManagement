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
    public class InventoriesController : ControllerBase
    {
        private readonly InventoryService _inventoryService;

        public InventoriesController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Create inventory entry - Pharmacist and Admin
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<InventoryResponseDto>> CreateInventory([FromBody] CreateInventoryDto dto)
        {
            try
            {
                var result = await _inventoryService.CreateInventoryAsync(dto);
                return CreatedAtAction(nameof(GetInventoryById), new { id = result.Id }, result);
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
        /// Get all inventories - Pharmacist and Admin
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<InventoryResponseDto>>> GetAllInventories()
        {
            try
            {
                var result = await _inventoryService.GetAllInventoriesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get inventory by ID - Pharmacist and Admin
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryResponseDto>> GetInventoryById(Guid id)
        {
            try
            {
                var result = await _inventoryService.GetInventoryByIdAsync(id);
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
        /// Get inventories by Medicine ID - Pharmacist and Admin
        /// </summary>
        [HttpGet("medicine/{medicineId}")]
        public async Task<ActionResult<List<InventoryResponseDto>>> GetInventoriesByMedicineId(Guid medicineId)
        {
            try
            {
                var result = await _inventoryService.GetInventoriesByMedicineIdAsync(medicineId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update inventory - Pharmacist and Admin
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<InventoryResponseDto>> UpdateInventory(Guid id, [FromBody] UpdateInventoryDto dto)
        {
            try
            {
                if (id != dto.Id)
                    return BadRequest(new { message = "ID mismatch" });

                var result = await _inventoryService.UpdateInventoryAsync(dto);
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
        /// Delete inventory - Admin only
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> DeleteInventory(Guid id)
        {
            try
            {
                var result = await _inventoryService.DeleteInventoryAsync(id);
                return Ok(new { message = "Inventory deleted successfully" });
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
