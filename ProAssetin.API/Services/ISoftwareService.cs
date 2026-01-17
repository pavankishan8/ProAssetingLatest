using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface ISoftwareService
    {
        Task<(IEnumerable<SoftwareDto> Software, int TotalCount)> GetSoftwareAsync(SoftwareQueryDto query, string tenantId);
        Task<SoftwareDto?> GetSoftwareByIdAsync(int id, string tenantId);
        Task<SoftwareDto> CreateSoftwareAsync(CreateSoftwareDto createDto, string tenantId, string userId);
        Task<SoftwareDto?> UpdateSoftwareAsync(int id, UpdateSoftwareDto updateDto, string tenantId);
        Task<bool> DeleteSoftwareAsync(int id, string tenantId);
        Task<IEnumerable<string>> GetCategoriesAsync(string tenantId);
        Task<IEnumerable<string>> GetLicenseTypesAsync();
        Task<IEnumerable<string>> GetStatusesAsync();
    }
}

