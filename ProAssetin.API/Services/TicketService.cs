using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public TicketService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<TicketDto> Items, int TotalCount)> GetTicketsAsync(TicketQueryDto query, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant();

            var q = context.Tickets.Where(t => t.TenantId.ToLower() == tid).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var t = query.SearchTerm;
                q = q.Where(x =>
                    x.TaskTitle.Contains(t) ||
                    (x.Description != null && x.Description.Contains(t)) ||
                    (x.TaskAssignedToName != null && x.TaskAssignedToName.Contains(t)) ||
                    (x.Resolution != null && x.Resolution.Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(query.TaskState))
                q = q.Where(x => x.TaskState == query.TaskState);

            if (!string.IsNullOrWhiteSpace(query.Priority))
                q = q.Where(x => x.Priority == query.Priority);

            var total = await q.CountAsync();

            q = query.SortBy?.ToLower() switch
            {
                "tasktitle" => query.SortDescending ? q.OrderByDescending(x => x.TaskTitle) : q.OrderBy(x => x.TaskTitle),
                "taskstate" => query.SortDescending ? q.OrderByDescending(x => x.TaskState) : q.OrderBy(x => x.TaskState),
                "priority" => query.SortDescending ? q.OrderByDescending(x => x.Priority) : q.OrderBy(x => x.Priority),
                "resolvedat" => query.SortDescending ? q.OrderByDescending(x => x.ResolvedAt) : q.OrderBy(x => x.ResolvedAt),
                _ => query.SortDescending ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt)
            };

            var rows = await q
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var userIds = rows.Where(x => !string.IsNullOrEmpty(x.CreatedByUserId)).Select(x => x.CreatedByUserId!).Distinct().ToList();
            var names = await ResolveUserNamesAsync(userIds);
            return (rows.Select(x => MapToDto(x, names)), total);
        }

        public async Task<TicketDto?> GetTicketByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant();

            var row = await context.Tickets
                .FirstOrDefaultAsync(t => t.TaskID == id && t.TenantId.ToLower() == tid);

            if (row == null) return null;

            var names = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, names);
        }

        public async Task<TicketDto> CreateTicketAsync(CreateTicketDto dto, string tenantId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant()!;

            var row = new Ticket
            {
                TaskTitle = dto.TaskTitle.Trim(),
                TaskAssignedToName = string.IsNullOrWhiteSpace(dto.TaskAssignedToName) ? null : dto.TaskAssignedToName.Trim(),
                TaskState = string.IsNullOrWhiteSpace(dto.TaskState) ? "Open" : dto.TaskState.Trim(),
                Priority = string.IsNullOrWhiteSpace(dto.Priority) ? "Medium" : dto.Priority.Trim(),
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                TenantId = tid,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.Tickets.Add(row);
            await context.SaveChangesAsync();

            var names = await ResolveUserNamesAsync(new List<string> { userId });
            return MapToDto(row, names);
        }

        public async Task<TicketDto?> UpdateTicketAsync(int id, UpdateTicketDto dto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant();

            var row = await context.Tickets
                .FirstOrDefaultAsync(t => t.TaskID == id && t.TenantId.ToLower() == tid);

            if (row == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.TaskTitle))
                row.TaskTitle = dto.TaskTitle.Trim();

            if (dto.TaskAssignedToName != null)
                row.TaskAssignedToName = string.IsNullOrWhiteSpace(dto.TaskAssignedToName) ? null : dto.TaskAssignedToName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.TaskState))
            {
                row.TaskState = dto.TaskState.Trim();
                if (row.TaskState is "Resolved" or "Closed" && row.ResolvedAt == null)
                    row.ResolvedAt = DateTime.UtcNow;
                if (row.TaskState is "Open" or "In Progress" or "Cancelled")
                    row.ResolvedAt = null;
            }

            if (dto.Priority != null)
                row.Priority = string.IsNullOrWhiteSpace(dto.Priority) ? null : dto.Priority.Trim();

            if (dto.Description != null)
                row.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();

            if (dto.Resolution != null)
                row.Resolution = string.IsNullOrWhiteSpace(dto.Resolution) ? null : dto.Resolution.Trim();

            row.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            var names = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, names);
        }

        public async Task<bool> DeleteTicketAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant();

            var row = await context.Tickets
                .FirstOrDefaultAsync(t => t.TaskID == id && t.TenantId.ToLower() == tid);

            if (row == null) return false;

            context.Tickets.Remove(row);
            await context.SaveChangesAsync();
            return true;
        }

        public Task<IEnumerable<string>> GetTaskStatesAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[]
            {
                "Open", "In Progress", "Resolved", "Closed", "Cancelled"
            });
        }

        public Task<IEnumerable<string>> GetPrioritiesAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[]
            {
                "Low", "Medium", "High", "Critical"
            });
        }

        private async Task<Dictionary<string, string>> ResolveUserNamesAsync(List<string> userIds)
        {
            if (!userIds.Any()) return new Dictionary<string, string>();

            var users = await _masterContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FirstName, u.LastName })
                .ToListAsync();

            return users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}".Trim());
        }

        private static TicketDto MapToDto(Ticket t, Dictionary<string, string> userNames)
        {
            string? createdName = null;
            if (!string.IsNullOrEmpty(t.CreatedByUserId))
                userNames.TryGetValue(t.CreatedByUserId, out createdName);

            return new TicketDto
            {
                Id = t.TaskID,
                TaskTitle = t.TaskTitle,
                TaskAssignedToName = t.TaskAssignedToName,
                TaskState = t.TaskState,
                Priority = t.Priority,
                Description = t.Description,
                Resolution = t.Resolution,
                CreatedByUserId = t.CreatedByUserId,
                CreatedByUserName = createdName,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
                ResolvedAt = t.ResolvedAt
            };
        }
    }
}
