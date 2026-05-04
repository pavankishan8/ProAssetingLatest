using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class ContractService : IContractService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public ContractService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<ContractDto> Items, int TotalCount)> GetContractsAsync(ContractQueryDto query, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant();

            var q = context.Contracts.Where(c => c.TenantId.ToLower() == tid).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var t = query.SearchTerm;
                q = q.Where(c =>
                    c.ContractReference.Contains(t) ||
                    c.Title.Contains(t) ||
                    (c.CounterpartyName != null && c.CounterpartyName.Contains(t)) ||
                    (c.ContractType != null && c.ContractType.Contains(t)) ||
                    (c.Notes != null && c.Notes.Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
                q = q.Where(c => c.Status == query.Status);

            if (!string.IsNullOrWhiteSpace(query.ContractType))
                q = q.Where(c => c.ContractType == query.ContractType);

            if (query.EndDateFrom.HasValue)
                q = q.Where(c => c.EndDate >= query.EndDateFrom.Value);

            if (query.EndDateTo.HasValue)
                q = q.Where(c => c.EndDate <= query.EndDateTo.Value);

            var total = await q.CountAsync();

            q = query.SortBy?.ToLower() switch
            {
                "title" => query.SortDescending ? q.OrderByDescending(c => c.Title) : q.OrderBy(c => c.Title),
                "contractreference" => query.SortDescending ? q.OrderByDescending(c => c.ContractReference) : q.OrderBy(c => c.ContractReference),
                "status" => query.SortDescending ? q.OrderByDescending(c => c.Status) : q.OrderBy(c => c.Status),
                "enddate" => query.SortDescending ? q.OrderByDescending(c => c.EndDate) : q.OrderBy(c => c.EndDate),
                "startdate" => query.SortDescending ? q.OrderByDescending(c => c.StartDate) : q.OrderBy(c => c.StartDate),
                "contractvalue" => query.SortDescending ? q.OrderByDescending(c => c.ContractValue) : q.OrderBy(c => c.ContractValue),
                _ => query.SortDescending ? q.OrderByDescending(c => c.EndDate).ThenByDescending(c => c.CreatedAt) : q.OrderByDescending(c => c.CreatedAt)
            };

            var rows = await q
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var userIds = rows.Where(c => !string.IsNullOrEmpty(c.CreatedByUserId)).Select(c => c.CreatedByUserId!).Distinct().ToList();
            var names = await ResolveUserNamesAsync(userIds);
            return (rows.Select(c => MapToDto(c, names)), total);
        }

        public async Task<ContractDto?> GetContractByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLower();

            var row = await context.Contracts
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId.ToLower() == tid);

            if (row == null) return null;

            var names = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, names);
        }

        public async Task<ContractDto> CreateContractAsync(CreateContractDto dto, string tenantId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLowerInvariant()!;

            var row = new Contract
            {
                ContractReference = dto.ContractReference,
                Title = dto.Title,
                CounterpartyName = dto.CounterpartyName,
                ContractType = dto.ContractType,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                RenewalReminderDate = dto.RenewalReminderDate,
                ContractValue = dto.ContractValue,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Draft" : dto.Status,
                Notes = dto.Notes,
                TenantId = tid,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.Contracts.Add(row);
            await context.SaveChangesAsync();

            var names = await ResolveUserNamesAsync(new List<string> { userId });
            return MapToDto(row, names);
        }

        public async Task<ContractDto?> UpdateContractAsync(int id, UpdateContractDto dto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLower();

            var row = await context.Contracts
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId.ToLower() == tid);

            if (row == null) return null;

            if (!string.IsNullOrWhiteSpace(dto.ContractReference))
                row.ContractReference = dto.ContractReference;

            if (!string.IsNullOrWhiteSpace(dto.Title))
                row.Title = dto.Title;

            if (dto.CounterpartyName != null)
                row.CounterpartyName = dto.CounterpartyName;

            if (dto.ContractType != null)
                row.ContractType = dto.ContractType;

            if (dto.StartDate.HasValue)
                row.StartDate = dto.StartDate;

            if (dto.EndDate.HasValue)
                row.EndDate = dto.EndDate;

            if (dto.RenewalReminderDate.HasValue)
                row.RenewalReminderDate = dto.RenewalReminderDate;

            if (dto.ContractValue.HasValue)
                row.ContractValue = dto.ContractValue;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                row.Status = dto.Status;

            if (dto.Notes != null)
                row.Notes = dto.Notes;

            row.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            var names = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, names);
        }

        public async Task<bool> DeleteContractAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var tid = tenantId?.ToLower();

            var row = await context.Contracts
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId.ToLower() == tid);

            if (row == null) return false;

            context.Contracts.Remove(row);
            await context.SaveChangesAsync();
            return true;
        }

        public Task<IEnumerable<string>> GetStatusesAsync()
        {
            return Task.FromResult<IEnumerable<string>>(new[]
            {
                "Draft", "Active", "Renewal Pending", "Expired", "Terminated"
            });
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

        private static ContractDto MapToDto(Contract c, Dictionary<string, string> userNames)
        {
            string? createdName = null;
            if (!string.IsNullOrEmpty(c.CreatedByUserId))
                userNames.TryGetValue(c.CreatedByUserId, out createdName);

            return new ContractDto
            {
                Id = c.Id,
                ContractReference = c.ContractReference,
                Title = c.Title,
                CounterpartyName = c.CounterpartyName,
                ContractType = c.ContractType,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                RenewalReminderDate = c.RenewalReminderDate,
                ContractValue = c.ContractValue,
                Status = c.Status,
                Notes = c.Notes,
                CreatedByUserId = c.CreatedByUserId,
                CreatedByUserName = createdName,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            };
        }
    }
}
