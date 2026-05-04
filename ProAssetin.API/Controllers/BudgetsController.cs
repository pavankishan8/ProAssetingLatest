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
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;
        private readonly ILogger<BudgetsController> _logger;

        public BudgetsController(IBudgetService budgetService, ILogger<BudgetsController> logger)
        {
            _budgetService = budgetService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetBudgets([FromQuery] BudgetQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _budgetService.GetBudgetsAsync(query, tenantId);

                Response.Headers.Append("X-Total-Count", result.TotalCount.ToString());
                Response.Headers.Append("X-Page-Number", query.PageNumber.ToString());
                Response.Headers.Append("X-Page-Size", query.PageSize.ToString());

                return Ok(new
                {
                    data = result.Budgets,
                    totalCount = result.TotalCount,
                    pageNumber = query.PageNumber,
                    pageSize = query.PageSize
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budgets for tenant {TenantId}", tenantId);
                return StatusCode(500, new { message = $"Error retrieving budgets: {ex.Message}" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBudget(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var budget = await _budgetService.GetBudgetByIdAsync(id, tenantId);
            if (budget == null)
            {
                return NotFound();
            }

            return Ok(budget);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDto createDto)
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

            var budget = await _budgetService.CreateBudgetAsync(createDto, tenantId, userId);
            return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] UpdateBudgetDto updateDto)
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

            var budget = await _budgetService.UpdateBudgetAsync(id, updateDto, tenantId);
            if (budget == null)
            {
                return NotFound();
            }

            return Ok(budget);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var deleted = await _budgetService.DeleteBudgetAsync(id, tenantId);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _budgetService.GetBudgetStatusesAsync();
            return Ok(statuses);
        }
    }
}
