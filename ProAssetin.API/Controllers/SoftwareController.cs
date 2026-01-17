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
    public class SoftwareController : ControllerBase
    {
        private readonly ISoftwareService _softwareService;

        public SoftwareController(ISoftwareService softwareService)
        {
            _softwareService = softwareService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSoftware([FromQuery] SoftwareQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var (software, totalCount) = await _softwareService.GetSoftwareAsync(query, tenantId);

            return Ok(new
            {
                software,
                totalCount,
                pageNumber = query.PageNumber,
                pageSize = query.PageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSoftware(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var software = await _softwareService.GetSoftwareByIdAsync(id, tenantId);

            if (software == null) return NotFound();

            return Ok(software);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSoftware([FromBody] CreateSoftwareDto createDto)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var software = await _softwareService.CreateSoftwareAsync(createDto, tenantId, userId);

            return CreatedAtAction(nameof(GetSoftware), new { id = software.Id }, software);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSoftware(int id, [FromBody] UpdateSoftwareDto updateDto)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var software = await _softwareService.UpdateSoftwareAsync(id, updateDto, tenantId);

            if (software == null) return NotFound();

            return Ok(software);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSoftware(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var deleted = await _softwareService.DeleteSoftwareAsync(id, tenantId);

            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var categories = await _softwareService.GetCategoriesAsync(tenantId);

            return Ok(categories);
        }

        [HttpGet("license-types")]
        public async Task<IActionResult> GetLicenseTypes()
        {
            var licenseTypes = await _softwareService.GetLicenseTypesAsync();

            return Ok(licenseTypes);
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _softwareService.GetStatusesAsync();

            return Ok(statuses);
        }
    }
}

