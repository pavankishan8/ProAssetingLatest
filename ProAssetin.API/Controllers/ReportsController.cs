using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProAssetin.API.Services;

namespace ProAssetin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportService reportService, ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetAssetSummary()
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var summary = await _reportService.GetAssetSummaryAsync(tenantId);
            return Ok(summary);
        }

        [HttpGet("category-stats")]
        public async Task<IActionResult> GetCategoryStats()
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var stats = await _reportService.GetAssetStatsByCategoryAsync(tenantId);
            return Ok(stats);
        }

        [HttpGet("status-stats")]
        public async Task<IActionResult> GetStatusStats()
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var stats = await _reportService.GetAssetStatsByStatusAsync(tenantId);
            return Ok(stats);
        }
    }
}

