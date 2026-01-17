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
    public class AssetsController : ControllerBase
    {
        private readonly IAssetService _assetService;
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<AssetsController> _logger;

        public AssetsController(
            IAssetService assetService,
            IInventoryService inventoryService,
            ILogger<AssetsController> logger)
        {
            _assetService = assetService;
            _inventoryService = inventoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAssets([FromQuery] AssetQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var result = await _assetService.GetAssetsAsync(query, tenantId);

            Response.Headers.Add("X-Total-Count", result.TotalCount.ToString());
            Response.Headers.Add("X-Page-Number", query.PageNumber.ToString());
            Response.Headers.Add("X-Page-Size", query.PageSize.ToString());

            return Ok(new
            {
                data = result.Assets,
                totalCount = result.TotalCount,
                pageNumber = query.PageNumber,
                pageSize = query.PageSize
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsset(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var asset = await _assetService.GetAssetByIdAsync(id, tenantId);
            if (asset == null)
            {
                return NotFound();
            }

            return Ok(asset);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsset([FromBody] CreateAssetDto createDto)
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

            var asset = await _assetService.CreateAssetAsync(createDto, tenantId, userId);

            // Log inventory action
            await _inventoryService.AddInventoryLogAsync(
                asset.Id,
                "Added",
                tenantId,
                userId,
                $"Asset {asset.AssetId} created");

            return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsset(int id, [FromBody] UpdateAssetDto updateDto)
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

            var asset = await _assetService.UpdateAssetAsync(id, updateDto, tenantId);
            if (asset == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _inventoryService.AddInventoryLogAsync(
                id,
                "Updated",
                tenantId,
                userId,
                $"Asset {asset.AssetId} updated");

            return Ok(asset);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsset(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var deleted = await _assetService.DeleteAssetAsync(id, tenantId);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var categories = await _assetService.GetCategoriesAsync(tenantId);
            return Ok(categories);
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await _assetService.GetStatusesAsync();
            return Ok(statuses);
        }

        [HttpPost("{id}/allocate")]
        [Authorize]
        public async Task<IActionResult> AllocateAsset(int id, [FromBody] AllocateAssetDto allocateDto)
        {
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

            // Update asset with assigned user and status
            var updateDto = new UpdateAssetDto
            {
                AssignedToUserId = allocateDto.UserId,
                Status = "In-Stock" // Or keep the current status, depending on requirement
            };

            var asset = await _assetService.UpdateAssetAsync(id, updateDto, tenantId);
            if (asset == null)
            {
                return NotFound();
            }

            // Log inventory action
            await _inventoryService.AddInventoryLogAsync(
                id,
                "Allocated",
                tenantId,
                userId,
                $"Asset {asset.AssetId} allocated to user {allocateDto.UserId}");

            return Ok(asset);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportAssetsFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded" });
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

            // Validate file extension
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Invalid file format. Only .xlsx and .xls files are allowed." });
            }

            try
            {
                var result = await _assetService.ImportAssetsFromExcelAsync(file, tenantId, userId);
                var count = result.Count();
                return Ok(new { message = $"Successfully imported {count} assets", importedCount = count, assets = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing assets from Excel");
                return BadRequest(new { message = $"Error importing assets: {ex.Message}" });
            }
        }
    }
}

