using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProAssetin.API.Models.DTOs;
using ProAssetin.API.Services;

namespace ProAssetin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VendorsController : ControllerBase
    {
        private readonly IVendorService _vendorService;

        public VendorsController(IVendorService vendorService)
        {
            _vendorService = vendorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetVendors([FromQuery] VendorQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var (vendors, totalCount) = await _vendorService.GetVendorsAsync(query, tenantId);

            return Ok(new
            {
                vendors,
                totalCount,
                pageNumber = query.PageNumber,
                pageSize = query.PageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVendor(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var vendor = await _vendorService.GetVendorByIdAsync(id, tenantId);

            if (vendor == null) return NotFound();

            return Ok(vendor);
        }

        [HttpPost]
        public async Task<IActionResult> CreateVendor([FromBody] CreateVendorDto createDto)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var vendor = await _vendorService.CreateVendorAsync(createDto, tenantId);

            return CreatedAtAction(nameof(GetVendor), new { id = vendor.Id }, vendor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendor(int id, [FromBody] UpdateVendorDto updateDto)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var vendor = await _vendorService.UpdateVendorAsync(id, updateDto, tenantId);

            if (vendor == null) return NotFound();

            return Ok(vendor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var deleted = await _vendorService.DeleteVendorAsync(id, tenantId);

            if (!deleted) return NotFound();

            return NoContent();
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveVendors()
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId)) return Unauthorized();

            var vendors = await _vendorService.GetActiveVendorsAsync(tenantId);

            return Ok(vendors);
        }
    }
}

