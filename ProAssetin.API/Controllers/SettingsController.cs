using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProAssetin.API.Models.DTOs;
using ProAssetin.API.Services;

namespace ProAssetin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanySettings()
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            try
            {
                var settings = await _settingsService.GetCompanySettingsAsync(tenantId);
                if (settings == null)
                {
                    return NotFound();
                }

                return Ok(settings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "An error occurred while loading settings", 
                    error = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCompanySettings([FromBody] UpdateCompanySettingsDto updateDto)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            try
            {
                var settings = await _settingsService.UpdateCompanySettingsAsync(updateDto, tenantId);
                return Ok(settings);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logo")]
        public async Task<IActionResult> UploadCompanyLogo(IFormFile logo)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            if (logo == null || logo.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
            }

            try
            {
                var success = await _settingsService.UploadCompanyLogoAsync(logo, tenantId);
                if (success)
                {
                    return Ok(new { message = "Logo uploaded successfully" });
                }
                return BadRequest(new { message = "Failed to upload logo" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}

