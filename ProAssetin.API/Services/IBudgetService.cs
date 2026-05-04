using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface IBudgetService
    {
        Task<(IEnumerable<BudgetDto> Budgets, int TotalCount)> GetBudgetsAsync(BudgetQueryDto query, string tenantId);
        Task<BudgetDto?> GetBudgetByIdAsync(int id, string tenantId);
        Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto createDto, string tenantId, string userId);
        Task<BudgetDto?> UpdateBudgetAsync(int id, UpdateBudgetDto updateDto, string tenantId);
        Task<bool> DeleteBudgetAsync(int id, string tenantId);
        Task<IEnumerable<string>> GetBudgetStatusesAsync();
    }
}
