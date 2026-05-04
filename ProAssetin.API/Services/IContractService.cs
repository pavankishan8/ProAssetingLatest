using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface IContractService
    {
        Task<(IEnumerable<ContractDto> Items, int TotalCount)> GetContractsAsync(ContractQueryDto query, string tenantId);
        Task<ContractDto?> GetContractByIdAsync(int id, string tenantId);
        Task<ContractDto> CreateContractAsync(CreateContractDto dto, string tenantId, string userId);
        Task<ContractDto?> UpdateContractAsync(int id, UpdateContractDto dto, string tenantId);
        Task<bool> DeleteContractAsync(int id, string tenantId);
        Task<IEnumerable<string>> GetStatusesAsync();
    }
}
