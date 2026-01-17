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
    }
}

