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
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly ILogger<ContractsController> _logger;

        public ContractsController(IContractService contractService, ILogger<ContractsController> logger)
        {
            _contractService = contractService;
            _logger = logger;
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            return Ok(await _contractService.GetStatusesAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetContracts([FromQuery] ContractQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            try
            {
                var result = await _contractService.GetContractsAsync(query, tenantId);
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
                _logger.LogError(ex, "Error listing contracts for {TenantId}", tenantId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetContract(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var item = await _contractService.GetContractByIdAsync(id, tenantId);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var created = await _contractService.CreateContractAsync(dto, tenantId, userId);
            return CreatedAtAction(nameof(GetContract), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateContract(int id, [FromBody] UpdateContractDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var updated = await _contractService.UpdateContractAsync(id, dto, tenantId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var ok = await _contractService.DeleteContractAsync(id, tenantId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
