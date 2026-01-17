using Microsoft.AspNetCore.Http;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface IAssetService
    {
        Task<(IEnumerable<AssetDto> Assets, int TotalCount)> GetAssetsAsync(AssetQueryDto query, string tenantId);
        Task<AssetDto?> GetAssetByIdAsync(int id, string tenantId);
        Task<AssetDto> CreateAssetAsync(CreateAssetDto createDto, string tenantId, string userId);
        Task<AssetDto?> UpdateAssetAsync(int id, UpdateAssetDto updateDto, string tenantId);
        Task<bool> DeleteAssetAsync(int id, string tenantId);
        Task<IEnumerable<string>> GetCategoriesAsync(string tenantId);
        Task<IEnumerable<string>> GetStatusesAsync();
        Task<IEnumerable<AssetDto>> ImportAssetsFromExcelAsync(IFormFile file, string tenantId, string userId);
    }
}

