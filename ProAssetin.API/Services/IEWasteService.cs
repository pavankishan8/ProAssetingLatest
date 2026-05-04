using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface IEWasteService
    {
        Task<(IEnumerable<EWasteDisposalDto> Items, int TotalCount)> GetDisposalsAsync(EWasteDisposalQueryDto query, string tenantId);
        Task<EWasteDisposalDto?> GetDisposalByIdAsync(int id, string tenantId);
        Task<EWasteDisposalDto> CreateDisposalAsync(CreateEWasteDisposalDto dto, string tenantId, string userId);
        Task<EWasteDisposalDto?> UpdateDisposalAsync(int id, UpdateEWasteDisposalDto dto, string tenantId);
        Task<bool> DeleteDisposalAsync(int id, string tenantId);
        Task<IEnumerable<string>> GetStatusesAsync();
    }
}
