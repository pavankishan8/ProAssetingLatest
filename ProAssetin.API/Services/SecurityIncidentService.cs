using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class SecurityIncidentService : ISecurityIncidentService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public SecurityIncidentService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<SecurityIncidentDto> Items, int TotalCount)> GetIncidentsAsync(SecurityIncidentQueryDto query, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant();

            var q = context.SecurityIncidents
                .Where(i => i.TenantId.ToLower() == tid)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var t = query.SearchTerm;
                q = q.Where(i =>
                    i.IncidentReference.Contains(t) ||
                    i.Title.Contains(t) ||
                    (i.Description != null && i.Description.Contains(t)) ||
                    (i.Category != null && i.Category.Contains(t)) ||
                    (i.AffectedSystem != null && i.AffectedSystem.Contains(t)) ||
                    (i.AssignedToName != null && i.AssignedToName.Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
                q = q.Where(i => i.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.Severity))
                q = q.Where(i => i.Severity == query.Severity);

            if (query.ReportedDateFrom.HasValue)
                q = q.Where(i => i.ReportedDate >= query.ReportedDateFrom.Value);

            if (query.ReportedDateTo.HasValue)
                q = q.Where(i => i.ReportedDate <= query.ReportedDateTo.Value);

            var total = await q.CountAsync();

            q = query.SortBy?.ToLower() switch
            {
                "title" => query.SortDescending ? q.OrderByDescending(i => i.Title) : q.OrderBy(i => i.Title),
                "severity" => query.SortDescending ? q.OrderByDescending(i => i.Severity) : q.OrderBy(i => i.Severity),
                "status" => query.SortDescending ? q.OrderByDescending(i => i.Status) : q.OrderBy(i => i.Status),
                "reporteddate" => query.SortDescending ? q.OrderByDescending(i => i.ReportedDate) : q.OrderBy(i => i.ReportedDate),
                "incidentreference" => query.SortDescending ? q.OrderByDescending(i => i.IncidentReference) : q.OrderBy(i => i.IncidentReference),
                _ => query.SortDescending ? q.OrderByDescending(i => i.ReportedDate) : q.OrderByDescending(i => i.ReportedDate)
            };

            var rows = await q
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var userIds = rows.Where(i => !string.IsNullOrEmpty(i.CreatedByUserId)).Select(i => i.CreatedByUserId!).Distinct().ToList();
            var names = await ResolveUserNamesAsync(userIds);
            return (rows.Select(i => MapToDto(i, names)), total);
        }

        public async Task<SecurityIncidentDto?> GetIncidentByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLower();

            var row = await context.SecurityIncidents
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId.ToLower() == tid);

            if (row == null) return null;

            var names = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, names);
        }

        public async Task<SecurityIncidentDto> CreateIncidentAsync(CreateSecurityIncidentDto dto, string tenantId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant()!;

            var row = new SecurityIncident
            {
                IncidentReference = dto.IncidentReference,
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                Severity = string.IsNullOrWhiteSpace(dto.Severity) ? "Medium" : dto.Severity,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Open" : dto.Status,
                ReportedDate = dto.ReportedDate,
                ResolvedDate = dto.ResolvedDate,
                AffectedSystem = dto.AffectedSystem,
                AssignedToName = dto.AssignedToName,
                Notes = dto.Notes,
                TenantId = tid,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.SecurityIncidents.Add(row);
            await context.SaveChangesAsync();

            var names = await ResolveUserNamesAsync(new List<string> { userId });
            return MapToDto(row, names);
        }

        public async Task<SecurityIncidentDto?> UpdateIncidentAsync(int id, UpdateSecurityIncidentDto dto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLower();

            var row = await context.SecurityIncidents
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId.ToLower() == tid);

            if (row == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.IncidentReference))
                row.IncidentReference = dto.IncidentReference;

            if (!string.IsNullOrWhiteSpace(dto.Title))
                row.Title = dto.Title;

            if (dto.Description != null)
                row.Description = dto.Description;

            if (dto.Category != null)
                row.Category = dto.Category;

            if (!string.IsNullOrWhiteSpace(dto.Severity))
                row.Severity = dto.Severity;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                row.Status = dto.Status;

            if (dto.ReportedDate.HasValue)
                row.ReportedDate = dto.ReportedDate.Value;

            if (dto.ResolvedDate.HasValue)
                row.ResolvedDate = dto.ResolvedDate;

            if (dto.AffectedSystem != null)
                row.AffectedSystem = dto.AffectedSystem;

            if (dto.AssignedToName != null)
                row.AssignedToName = dto.AssignedToName;

            if (dto.Notes != null)
                row.Notes = dto.Notes;

            row.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            var names = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, names);
        }

        public async Task<bool> DeleteIncidentAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLower();

            var row = await context.SecurityIncidents
                .FirstOrDefaultAsync(i => i.Id == id && i.TenantId.ToLower() == tid);

            if (row == null) return false;

            context.SecurityIncidents.Remove(row);
            await context.SaveChangesAsync();
            return true;
        }

        public Task<IEnumerable<string>> GetStatusesAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[] { "Open", "Investigating", "Mitigated", "Closed" });
        }

        public Task<IEnumerable<string>> GetSeveritiesAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[] { "Low", "Medium", "High", "Critical" });
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

        private static SecurityIncidentDto MapToDto(SecurityIncident i, Dictionary<string, string> userNames)
        {
            string? createdName = null;
            if (!string.IsNullOrEmpty(i.CreatedByUserId))
                userNames.TryGetValue(i.CreatedByUserId, out createdName);

            return new SecurityIncidentDto
            {
                Id = i.Id,
                IncidentReference = i.IncidentReference,
                Title = i.Title,
                Description = i.Description,
                Category = i.Category,
                Severity = i.Severity,
                Status = i.Status,
                ReportedDate = i.ReportedDate,
                ResolvedDate = i.ResolvedDate,
                AffectedSystem = i.AffectedSystem,
                AssignedToName = i.AssignedToName,
                Notes = i.Notes,
                CreatedByUserId = i.CreatedByUserId,
                CreatedByUserName = createdName,
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt
            };
        }
    }
}
