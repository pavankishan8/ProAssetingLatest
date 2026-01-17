using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;

namespace ProAssetin.API.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;

        public InventoryService(ITenantDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<IEnumerable<InventoryLog>> GetInventoryLogsAsync(int assetId, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            return await context.InventoryLogs
                .Where(log => log.AssetId == assetId && log.TenantId == tenantId)
                .Include(log => log.PerformedByUser)
                .OrderByDescending(log => log.CreatedAt)
                .ToListAsync();
        }

        public async Task<InventoryLog> AddInventoryLogAsync(int assetId, string action, string tenantId, string? userId, string? notes)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var log = new InventoryLog
            {
                AssetId = assetId,
                Action = action,
                TenantId = tenantId,
                PerformedByUserId = userId,
                Notes = notes,
                CreatedAt = DateTime.UtcNow
            };

            context.InventoryLogs.Add(log);
            await context.SaveChangesAsync();

            return log;
        }
    }
}

