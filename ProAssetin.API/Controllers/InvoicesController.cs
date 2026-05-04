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
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<InvoicesController> _logger;

        public InvoicesController(IInvoiceService invoiceService, ILogger<InvoicesController> logger)
        {
            _invoiceService = invoiceService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetInvoices([FromQuery] InvoiceQueryDto query)
        {
            try
            {
                var tenantId = HttpContext.Items["TenantId"]?.ToString();
                if (string.IsNullOrEmpty(tenantId))
                {
                    _logger.LogWarning("GetInvoices: TenantId not found in HttpContext");
                    return Unauthorized();
                }

                _logger.LogInformation("Getting invoices for tenant: {TenantId}, Page: {PageNumber}, PageSize: {PageSize}",
                    tenantId, query.PageNumber, query.PageSize);

                (IEnumerable<InvoiceDto> Invoices, int TotalCount) result;
                try
                {
                    result = await _invoiceService.GetInvoicesAsync(query, tenantId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting invoices for tenant {TenantId}", tenantId);
                    return StatusCode(500, new { message = $"Error retrieving invoices: {ex.Message}" });
                }

                _logger.LogInformation("Found {Count} invoices for tenant {TenantId}", result.TotalCount, tenantId);

                try
                {
                    Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
                    Response.Headers.Add("X-Page-Number", query.PageNumber.ToString());
                    Response.Headers.Add("X-Page-Size", query.PageSize.ToString());
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error adding response headers");
                }

                return Ok(new
                {
                    data = result.Invoices,
                    totalCount = result.TotalCount,
                    pageNumber = query.PageNumber,
                    pageSize = query.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetInvoices endpoint");
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.GetInvoiceByIdAsync(id, tenantId);
            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.CreateInvoiceAsync(createDto, tenantId, userId);

            return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateInvoice(int id, [FromBody] UpdateInvoiceDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var invoice = await _invoiceService.UpdateInvoiceAsync(id, updateDto, tenantId);
            if (invoice == null)
            {
                return NotFound();
            }

            return Ok(invoice);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var deleted = await _invoiceService.DeleteInvoiceAsync(id, tenantId);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _invoiceService.GetInvoiceStatusesAsync();
            return Ok(statuses);
        }
    }
}

