using Microsoft.AspNetCore.Http;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface ISettingsService
    {
        Task<CompanySettingsDto?> GetCompanySettingsAsync(string tenantId);
        Task<CompanySettingsDto> UpdateCompanySettingsAsync(UpdateCompanySettingsDto updateDto, string tenantId);
        Task<bool> UploadCompanyLogoAsync(IFormFile logoFile, string tenantId);
    }
}

