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
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            return Ok(await _projectService.GetStatusesAsync());
        }

        [HttpGet("priorities")]
        public async Task<IActionResult> GetPriorities()
        {
            return Ok(await _projectService.GetPrioritiesAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] ProjectQueryDto query)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            try
            {
                var result = await _projectService.GetProjectsAsync(query, tenantId);
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
                _logger.LogError(ex, "Error listing projects for {TenantId}", tenantId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var item = await _projectService.GetProjectByIdAsync(id, tenantId);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var created = await _projectService.CreateProjectAsync(dto, tenantId, userId);
            return CreatedAtAction(nameof(GetProject), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var updated = await _projectService.UpdateProjectAsync(id, dto, tenantId);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
                return Unauthorized();

            var ok = await _projectService.DeleteProjectAsync(id, tenantId);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
