using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface IVendorService
    {
        Task<(IEnumerable<VendorDto> Vendors, int TotalCount)> GetVendorsAsync(VendorQueryDto query, string tenantId);
        Task<VendorDto?> GetVendorByIdAsync(int id, string tenantId);
        Task<VendorDto> CreateVendorAsync(CreateVendorDto createDto, string tenantId);
        Task<VendorDto?> UpdateVendorAsync(int id, UpdateVendorDto updateDto, string tenantId);
        Task<bool> DeleteVendorAsync(int id, string tenantId);
        Task<IEnumerable<VendorDto>> GetActiveVendorsAsync(string tenantId);
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync(string tenantId);
    }
}

