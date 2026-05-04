using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;

namespace ProAssetin.API.Services
{
    public class ReportService : IReportService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;

        public ReportService(ITenantDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<object> GetAssetSummaryAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Normalize tenant ID to lowercase (should already be lowercase from TenantResolver, but ensure it)
            var normalizedTenantId = tenantId?.ToLowerInvariant() ?? string.Empty;

            // Use direct comparison - both tenantId in DB and normalizedTenantId should be lowercase
            var totalAssets = await context.Assets.CountAsync(a => a.TenantId == normalizedTenantId);
            var totalValue = await context.Assets
                .Where(a => a.TenantId == normalizedTenantId && a.PurchasePrice.HasValue)
                .SumAsync(a => a.PurchasePrice!.Value);

            var inStockAssets = await context.Assets.CountAsync(a => a.TenantId == normalizedTenantId && a.Status == "In-Stock");
            var repairAssets = await context.Assets.CountAsync(a => a.TenantId == normalizedTenantId && a.Status == "Repair");
            var soldAssets = await context.Assets.CountAsync(a => a.TenantId == normalizedTenantId && a.Status == "Sold");
            var damagedAssets = await context.Assets.CountAsync(a => a.TenantId == normalizedTenantId && a.Status == "Damaged");
            var ewasteAssets = await context.Assets.CountAsync(a => a.TenantId == normalizedTenantId && a.Status == "E-Waste");

            return new
            {
                totalAssets = totalAssets,
                totalValue = totalValue,
                inStockAssets = inStockAssets,
                repairAssets = repairAssets,
                soldAssets = soldAssets,
                damagedAssets = damagedAssets,
                ewasteAssets = ewasteAssets,
                // Keep old property names for backward compatibility with existing dashboard
                allocatedAssets = inStockAssets,
                availableAssets = inStockAssets
            };
        }

        public async Task<object> GetAssetStatsByCategoryAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Normalize tenant ID to lowercase (should already be lowercase from TenantResolver, but ensure it)
            var normalizedTenantId = tenantId?.ToLowerInvariant() ?? string.Empty;

            var stats = await context.Assets
                .Where(a => a.TenantId == normalizedTenantId)
                .GroupBy(a => a.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return stats;
        }

        public async Task<object> GetAssetStatsByStatusAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Normalize tenant ID to lowercase (should already be lowercase from TenantResolver, but ensure it)
            var normalizedTenantId = tenantId?.ToLowerInvariant() ?? string.Empty;

            var stats = await context.Assets
                .Where(a => a.TenantId == normalizedTenantId)
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return stats;
        }

        public async Task<object> GetAssetAdditionsWeeklyAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLowerInvariant() ?? string.Empty;
            var today = DateTime.UtcNow.Date;
            var sevenDaysAgo = today.AddDays(-6);

            var allAssets = await context.Assets
                .Where(a => a.TenantId == normalizedTenantId && a.CreatedAt >= sevenDaysAgo)
                .ToListAsync();

            // Group assets by date
            var weeklyData = allAssets
                .GroupBy(a => a.CreatedAt.Date)
                .Select(g => new
                {
                    Day = g.Key.ToString("yyyy-MM-dd"),
                    DayName = g.Key.ToString("ddd"),
                    Count = g.Count()
                })
                .OrderBy(x => x.Day)
                .ToList();

            // Ensure all 7 days are represented
            var result = new List<object>();
            for (int i = 0; i < 7; i++)
            {
                var date = sevenDaysAgo.AddDays(i);
                var dateStr = date.ToString("yyyy-MM-dd");
                var dayName = date.ToString("ddd");
                var existing = weeklyData.FirstOrDefault(w => w.Day == dateStr);
                result.Add(new
                {
                    Day = dateStr,
                    DayName = dayName,
                    Count = existing?.Count ?? 0
                });
            }

            return result;
        }

        public async Task<object> GetAssetAdditionsMonthlyAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLowerInvariant() ?? string.Empty;
            var today = DateTime.UtcNow.Date;
            var sixMonthsAgo = today.AddMonths(-6).Date;

            var monthlyData = await context.Assets
                .Where(a => a.TenantId == normalizedTenantId && a.CreatedAt >= sixMonthsAgo)
                .GroupBy(a => new { Year = a.CreatedAt.Year, Month = a.CreatedAt.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"),
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            // Ensure all 6 months are represented
            var result = new List<object>();
            for (int i = 5; i >= 0; i--)
            {
                var date = today.AddMonths(-i);
                var monthName = date.ToString("MMM yyyy");
                var existing = monthlyData.FirstOrDefault(m => m.Year == date.Year && m.Month == date.Month);
                result.Add(new
                {
                    Year = date.Year,
                    Month = date.Month,
                    MonthName = monthName,
                    Count = existing?.Count ?? 0
                });
            }

            return result;
        }
    }
}

