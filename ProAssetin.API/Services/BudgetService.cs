using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public BudgetService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<BudgetDto> Budgets, int TotalCount)> GetBudgetsAsync(BudgetQueryDto query, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLowerInvariant();

            var budgetsQuery = context.Budgets
                .Where(b => b.TenantId != null && b.TenantId.ToLower() == normalizedTenantId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                budgetsQuery = budgetsQuery.Where(b =>
                    b.Name.Contains(query.SearchTerm) ||
                    (b.Description != null && b.Description.Contains(query.SearchTerm)) ||
                    (b.Category != null && b.Category.Contains(query.SearchTerm)));
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                budgetsQuery = budgetsQuery.Where(b => b.Status == query.Status);
            }

            if (query.FiscalYear.HasValue)
            {
                budgetsQuery = budgetsQuery.Where(b => b.FiscalYear == query.FiscalYear.Value);
            }

            var totalCount = await budgetsQuery.CountAsync();

            budgetsQuery = query.SortBy?.ToLower() switch
            {
                "name" => query.SortDescending ? budgetsQuery.OrderByDescending(b => b.Name) : budgetsQuery.OrderBy(b => b.Name),
                "fiscalyear" => query.SortDescending ? budgetsQuery.OrderByDescending(b => b.FiscalYear) : budgetsQuery.OrderBy(b => b.FiscalYear),
                "allocatedamount" => query.SortDescending ? budgetsQuery.OrderByDescending(b => b.AllocatedAmount) : budgetsQuery.OrderBy(b => b.AllocatedAmount),
                "spentamount" => query.SortDescending ? budgetsQuery.OrderByDescending(b => b.SpentAmount) : budgetsQuery.OrderBy(b => b.SpentAmount),
                "status" => query.SortDescending ? budgetsQuery.OrderByDescending(b => b.Status) : budgetsQuery.OrderBy(b => b.Status),
                _ => query.SortDescending ? budgetsQuery.OrderByDescending(b => b.FiscalYear).ThenByDescending(b => b.Name) : budgetsQuery.OrderByDescending(b => b.FiscalYear).ThenBy(b => b.Name)
            };

            var budgets = await budgetsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var userIds = budgets
                .Where(b => !string.IsNullOrEmpty(b.CreatedByUserId))
                .Select(b => b.CreatedByUserId!)
                .Distinct()
                .ToList();

            Dictionary<string, string> userNamesDict = new();
            if (userIds.Any())
            {
                var users = await _masterContext.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.FirstName, u.LastName })
                    .ToListAsync();

                userNamesDict = users.ToDictionary(
                    u => u.Id,
                    u => $"{u.FirstName} {u.LastName}");
            }

            var dtos = budgets.Select(b => MapToDto(b, userNamesDict));
            return (dtos, totalCount);
        }

        public async Task<BudgetDto?> GetBudgetByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLower();

            var budget = await context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.TenantId.ToLower() == normalizedTenantId);

            if (budget == null) return null;

            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(budget.CreatedByUserId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == budget.CreatedByUserId)
                    .Select(u => new { u.Id, u.FirstName, u.LastName })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    userNamesDict = new Dictionary<string, string>
                    {
                        { user.Id, $"{user.FirstName} {user.LastName}" }
                    };
                }
            }

            return MapToDto(budget, userNamesDict);
        }

        public async Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto createDto, string tenantId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLowerInvariant();

            var budget = new Budget
            {
                Name = createDto.Name,
                Description = createDto.Description,
                FiscalYear = createDto.FiscalYear,
                Category = createDto.Category,
                AllocatedAmount = createDto.AllocatedAmount,
                SpentAmount = createDto.SpentAmount < 0 ? 0 : createDto.SpentAmount,
                Status = string.IsNullOrWhiteSpace(createDto.Status) ? "Active" : createDto.Status,
                TenantId = normalizedTenantId!,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.Budgets.Add(budget);
            await context.SaveChangesAsync();

            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == userId)
                    .Select(u => new { u.Id, u.FirstName, u.LastName })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    userNamesDict = new Dictionary<string, string>
                    {
                        { user.Id, $"{user.FirstName} {user.LastName}" }
                    };
                }
            }

            return MapToDto(budget, userNamesDict);
        }

        public async Task<BudgetDto?> UpdateBudgetAsync(int id, UpdateBudgetDto updateDto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLower();

            var budget = await context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.TenantId.ToLower() == normalizedTenantId);

            if (budget == null) return null;

            if (!string.IsNullOrWhiteSpace(updateDto.Name))
                budget.Name = updateDto.Name;

            if (updateDto.Description != null)
                budget.Description = updateDto.Description;

            if (updateDto.FiscalYear.HasValue)
                budget.FiscalYear = updateDto.FiscalYear.Value;

            if (updateDto.Category != null)
                budget.Category = updateDto.Category;

            if (updateDto.AllocatedAmount.HasValue)
                budget.AllocatedAmount = updateDto.AllocatedAmount.Value;

            if (updateDto.SpentAmount.HasValue)
                budget.SpentAmount = updateDto.SpentAmount.Value < 0 ? 0 : updateDto.SpentAmount.Value;

            if (!string.IsNullOrWhiteSpace(updateDto.Status))
                budget.Status = updateDto.Status;

            budget.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(budget.CreatedByUserId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == budget.CreatedByUserId)
                    .Select(u => new { u.Id, u.FirstName, u.LastName })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    userNamesDict = new Dictionary<string, string>
                    {
                        { user.Id, $"{user.FirstName} {user.LastName}" }
                    };
                }
            }

            return MapToDto(budget, userNamesDict);
        }

        public async Task<bool> DeleteBudgetAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLower();

            var budget = await context.Budgets
                .FirstOrDefaultAsync(b => b.Id == id && b.TenantId.ToLower() == normalizedTenantId);

            if (budget == null) return false;

            context.Budgets.Remove(budget);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<string>> GetBudgetStatusesAsync()
        {
            return await Task.FromResult(new[] { "Active", "Closed" });
        }

        private static BudgetDto MapToDto(Budget budget, Dictionary<string, string>? userNamesDict = null)
        {
            string? createdByUserName = null;
            if (!string.IsNullOrEmpty(budget.CreatedByUserId) && userNamesDict != null)
            {
                userNamesDict.TryGetValue(budget.CreatedByUserId, out createdByUserName);
            }

            var remaining = budget.AllocatedAmount - budget.SpentAmount;

            return new BudgetDto
            {
                Id = budget.Id,
                Name = budget.Name,
                Description = budget.Description,
                FiscalYear = budget.FiscalYear,
                Category = budget.Category,
                AllocatedAmount = budget.AllocatedAmount,
                SpentAmount = budget.SpentAmount,
                RemainingAmount = remaining,
                Status = budget.Status,
                CreatedByUserId = budget.CreatedByUserId,
                CreatedByUserName = createdByUserName,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            };
        }
    }
}
