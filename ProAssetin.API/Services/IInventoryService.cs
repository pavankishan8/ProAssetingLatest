using ProAssetin.API.Models;

namespace ProAssetin.API.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<InventoryLog>> GetInventoryLogsAsync(int assetId, string tenantId);
        Task<InventoryLog> AddInventoryLogAsync(int assetId, string action, string tenantId, string? userId, string? notes);
    }
}

