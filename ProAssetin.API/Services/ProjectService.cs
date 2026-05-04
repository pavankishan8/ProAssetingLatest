using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class ProjectService : IProjectService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public ProjectService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<ProjectDto> Items, int TotalCount)> GetProjectsAsync(ProjectQueryDto query, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant();

            var q = context.Projects.Where(p => p.TenantId.ToLower() == tid).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var t = query.SearchTerm;
                q = q.Where(p =>
                    p.ProjectReference.Contains(t) ||
                    p.Name.Contains(t) ||
                    (p.Description != null && p.Description.Contains(t)) ||
                    (p.ProjectManagerName != null && p.ProjectManagerName.Contains(t)) ||
                    (p.DepartmentOrClient != null && p.DepartmentOrClient.Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
                q = q.Where(p => p.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.Priority))
                q = q.Where(p => p.Priority == query.Priority);

            if (query.StartDateFrom.HasValue)
                q = q.Where(p => p.StartDate >= query.StartDateFrom.Value);

            if (query.StartDateTo.HasValue)
                q = q.Where(p => p.StartDate <= query.StartDateTo.Value);

            var total = await q.CountAsync();

            q = query.SortBy?.ToLower() switch
            {
                "name" => query.SortDescending ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name),
                "projectreference" => query.SortDescending ? q.OrderByDescending(p => p.ProjectReference) : q.OrderBy(p => p.ProjectReference),
                "status" => query.SortDescending ? q.OrderByDescending(p => p.Status) : q.OrderBy(p => p.Status),
                "priority" => query.SortDescending ? q.OrderByDescending(p => p.Priority) : q.OrderBy(p => p.Priority),
                "startdate" => query.SortDescending ? q.OrderByDescending(p => p.StartDate) : q.OrderBy(p => p.StartDate),
                "enddate" => query.SortDescending ? q.OrderByDescending(p => p.EndDate) : q.OrderBy(p => p.EndDate),
                _ => query.SortDescending ? q.OrderByDescending(p => p.CreatedAt) : q.OrderByDescending(p => p.CreatedAt)
            };

            var rows = await q
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var userIds = rows.Where(p => !string.IsNullOrEmpty(p.CreatedByUserId)).Select(p => p.CreatedByUserId!).Distinct().ToList();
            var names = await ResolveUserNamesAsync(userIds);
            return (rows.Select(p => MapToDto(p, names)), total);
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLower();

            var row = await context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.TenantId.ToLower() == tid);

            if (row == null) return null;

            var names = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, names);
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, string tenantId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant()!;

            var row = new Project
            {
                ProjectReference = dto.ProjectReference,
                Name = dto.Name,
                Description = dto.Description,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Planning" : dto.Status,
                Priority = string.IsNullOrWhiteSpace(dto.Priority) ? "Medium" : dto.Priority,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ProjectManagerName = dto.ProjectManagerName,
                DepartmentOrClient = dto.DepartmentOrClient,
                Notes = dto.Notes,
                TenantId = tid,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.Projects.Add(row);
            await context.SaveChangesAsync();

            var names = await ResolveUserNamesAsync(new List<string> { userId });
            return MapToDto(row, names);
        }

        public async Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto dto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLower();

            var row = await context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.TenantId.ToLower() == tid);

            if (row == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.ProjectReference))
                row.ProjectReference = dto.ProjectReference;

            if (!string.IsNullOrWhiteSpace(dto.Name))
                row.Name = dto.Name;

            if (dto.Description != null)
                row.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                row.Status = dto.Status;

            if (!string.IsNullOrWhiteSpace(dto.Priority))
                row.Priority = dto.Priority;

            if (dto.StartDate.HasValue)
                row.StartDate = dto.StartDate;

            if (dto.EndDate.HasValue)
                row.EndDate = dto.EndDate;

            if (dto.ProjectManagerName != null)
                row.ProjectManagerName = dto.ProjectManagerName;

            if (dto.DepartmentOrClient != null)
                row.DepartmentOrClient = dto.DepartmentOrClient;

            if (dto.Notes != null)
                row.Notes = dto.Notes;

            row.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            var names = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, names);
        }

        public async Task<bool> DeleteProjectAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLower();

            var row = await context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.TenantId.ToLower() == tid);

            if (row == null) return false;

            context.Projects.Remove(row);
            await context.SaveChangesAsync();
            return true;
        }

        public Task<IEnumerable<string>> GetStatusesAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[]
            {
                "Planning", "Active", "On Hold", "Completed", "Cancelled"
            });
        }

        public Task<IEnumerable<string>> GetPrioritiesAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[] { "Low", "Medium", "High" });
        }

        private async Task<Dictionary<string, string>> ResolveUserNamesAsync(List<string> userIds)
        {
            if (!userIds.Any()) return new Dictionary<string, string>();

            var users = await _masterContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FirstName, u.LastName })
                .ToListAsync();

            return users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");
        }

        private static ProjectDto MapToDto(Project p, Dictionary<string, string> userNames)
        {
            string? createdName = null;
            if (!string.IsNullOrEmpty(p.CreatedByUserId))
                userNames.TryGetValue(p.CreatedByUserId, out createdName);

            return new ProjectDto
            {
                Id = p.Id,
                ProjectReference = p.ProjectReference,
                Name = p.Name,
                Description = p.Description,
                Status = p.Status,
                Priority = p.Priority,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                ProjectManagerName = p.ProjectManagerName,
                DepartmentOrClient = p.DepartmentOrClient,
                Notes = p.Notes,
                CreatedByUserId = p.CreatedByUserId,
                CreatedByUserName = createdName,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}
