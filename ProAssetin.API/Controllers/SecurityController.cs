using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProAssetin.API.Models.DTOs;
using ProAssetin.API.Services;
using System.Security.Claims;

namespace ProAssetin.API.Controllers
{
    [ApiController]
    [Route("api/security")]
    [Authorize]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityIncidentService _securityService;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(ISecurityIncidentService securityService, ILogger<SecurityController> logger)
        {
            _securityService = securityService;
            _logger = logger;
        }

        [HttpGet("incidents/statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            return Ok(await _securityService.GetStatusesAsync());
        }

        [HttpGet("incidents/severities")]
        public async Task<IActionResult> GetSeverities()
        {
            return Ok(await _securityService.GetSeveritiesAsync());
        }

        [HttpGet("incidents")]
        public async Task<IActionResult> GetIncidents([FromQuery] SecurityIncidentQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            try
            {
                var result = await _securityService.GetIncidentsAsync(query, tenantId);
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
                _logger.LogError(ex, "Error listing security incidents for {TenantId}", tenantId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("incidents/{id:int}")]
        public async Task<IActionResult> GetIncident(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var item = await _securityService.GetIncidentByIdAsync(id, tenantId);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost("incidents")]
        public async Task<IActionResult> CreateIncident([FromBody] CreateSecurityIncidentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var created = await _securityService.CreateIncidentAsync(dto, tenantId, userId);
            return CreatedAtAction(nameof(GetIncident), new { id = created.Id }, created);
        }

        [HttpPut("incidents/{id:int}")]
        public async Task<IActionResult> UpdateIncident(int id, [FromBody] UpdateSecurityIncidentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var updated = await _securityService.UpdateIncidentAsync(id, dto, tenantId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("incidents/{id:int}")]
        public async Task<IActionResult> DeleteIncident(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var ok = await _securityService.DeleteIncidentAsync(id, tenantId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
