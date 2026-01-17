using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface IPurchaseOrderService
    {
        Task<(IEnumerable<PurchaseOrderDto> PurchaseOrders, int TotalCount)> GetPurchaseOrdersAsync(PurchaseOrderQueryDto query, string tenantId);
        Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(int id, string tenantId);
        Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto createDto, string tenantId, string userId);
        Task<PurchaseOrderDto?> UpdatePurchaseOrderAsync(int id, UpdatePurchaseOrderDto updateDto, string tenantId);
        Task<bool> DeletePurchaseOrderAsync(int id, string tenantId);
        Task<PurchaseOrderDto?> ApprovePurchaseOrderAsync(int id, string tenantId, string approvedByUserId);
    }
}

