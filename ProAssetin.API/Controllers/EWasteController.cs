using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProAssetin.API.Models.DTOs;
using ProAssetin.API.Services;
using System.Security.Claims;

namespace ProAssetin.API.Controllers
{
    [ApiController]
    [Route("api/ewaste")]
    [Authorize]
    public class EWasteController : ControllerBase
    {
        private readonly IEWasteService _ewasteService;
        private readonly ILogger<EWasteController> _logger;

        public EWasteController(IEWasteService ewasteService, ILogger<EWasteController> logger)
        {
            _ewasteService = ewasteService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDisposals([FromQuery] EWasteDisposalQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            try
            {
                var result = await _ewasteService.GetDisposalsAsync(query, tenantId);
                Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
                Response.Headers.Append("X-Page-Number", query.PageNumber.ToString());
                Response.Headers.Append("X-Page-Size", query.PageSize.ToString());

                return Ok(new
                {
                    data = result.Items,
                    totalCount = result.TotalCount,
                    pageNumber = query.PageNumber,
                    pageSize = query.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing e-waste for tenant {TenantId}", tenantId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDisposal(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var item = await _ewasteService.GetDisposalByIdAsync(id, tenantId);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDisposal([FromBody] CreateEWasteDisposalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            try
            {
                var created = await _ewasteService.CreateDisposalAsync(dto, tenantId, userId);
                return CreatedAtAction(nameof(GetDisposal), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDisposal(int id, [FromBody] UpdateEWasteDisposalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            try
            {
                var updated = await _ewasteService.UpdateDisposalAsync(id, dto, tenantId);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDisposal(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var ok = await _ewasteService.DeleteDisposalAsync(id, tenantId);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            return Ok(await _ewasteService.GetStatusesAsync());
        }
    }
}
