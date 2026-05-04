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
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketService ticketService, ILogger<TicketsController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpGet("states")]
        public async Task<IActionResult> GetTaskStates()
        {
            return Ok(await _ticketService.GetTaskStatesAsync());
        }

        [HttpGet("priorities")]
        public async Task<IActionResult> GetPriorities()
        {
            return Ok(await _ticketService.GetPrioritiesAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] TicketQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            try
            {
                var result = await _ticketService.GetTicketsAsync(query, tenantId);
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
                _logger.LogError(ex, "Error listing tickets for {TenantId}", tenantId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var item = await _ticketService.GetTicketByIdAsync(id, tenantId);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var created = await _ticketService.CreateTicketAsync(dto, tenantId, userId);
            return CreatedAtAction(nameof(GetTicket), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] UpdateTicketDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var updated = await _ticketService.UpdateTicketAsync(id, dto, tenantId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var ok = await _ticketService.DeleteTicketAsync(id, tenantId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
