using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProAssetin.API.Models.DTOs;
using ProAssetin.API.Services;
using System.Security.Claims;

namespace ProAssetin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;

        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPurchaseOrders([FromQuery] PurchaseOrderQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var (purchaseOrders, totalCount) = await _purchaseOrderService.GetPurchaseOrdersAsync(query, tenantId);

            return Ok(new
            {
                purchaseOrders,
                totalCount,
                pageNumber = query.PageNumber,
                pageSize = query.PageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrder(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var purchaseOrder = await _purchaseOrderService.GetPurchaseOrderByIdAsync(id, tenantId);

            if (purchaseOrder == null) return NotFound();

            return Ok(purchaseOrder);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] CreatePurchaseOrderDto createDto)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var purchaseOrder = await _purchaseOrderService.CreatePurchaseOrderAsync(createDto, tenantId, userId);

            return CreatedAtAction(nameof(GetPurchaseOrder), new { id = purchaseOrder.Id }, purchaseOrder);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int id, [FromBody] UpdatePurchaseOrderDto updateDto)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var purchaseOrder = await _purchaseOrderService.UpdatePurchaseOrderAsync(id, updateDto, tenantId);

            if (purchaseOrder == null) return NotFound();

            return Ok(purchaseOrder);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var deleted = await _purchaseOrderService.DeletePurchaseOrderAsync(id, tenantId);

            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApprovePurchaseOrder(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var approvedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(approvedByUserId)) return Unauthorized();

            var purchaseOrder = await _purchaseOrderService.ApprovePurchaseOrderAsync(id, tenantId, approvedByUserId);

            if (purchaseOrder == null) return NotFound();

            return Ok(purchaseOrder);
        }
    }
}

