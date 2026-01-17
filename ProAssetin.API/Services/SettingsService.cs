using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;

        public SettingsService(ITenantDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<CompanySettingsDto?> GetCompanySettingsAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Ensure database exists
            await context.Database.EnsureCreatedAsync();

            // Normalize tenant ID to lowercase for case-insensitive comparison
            var normalizedTenantId = tenantId?.ToLower();

            var settings = await context.CompanySettings
                .FirstOrDefaultAsync(s => s.TenantId.ToLower() == normalizedTenantId);

            if (settings == null)
            {
                // Create default settings if none exist
                return await CreateDefaultSettingsAsync(tenantId);
            }

            return MapToDto(settings);
        }

        public async Task<CompanySettingsDto> UpdateCompanySettingsAsync(UpdateCompanySettingsDto updateDto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Ensure database exists
            await context.Database.EnsureCreatedAsync();

            // Normalize tenant ID to lowercase for case-insensitive comparison
            var normalizedTenantId = tenantId?.ToLower();

            var settings = await context.CompanySettings
                .FirstOrDefaultAsync(s => s.TenantId.ToLower() == normalizedTenantId);

            if (settings == null)
            {
                // Create new settings if none exist
                settings = new CompanySettings
                {
                    TenantId = normalizedTenantId
                };
                context.CompanySettings.Add(settings);
            }

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(updateDto.CompanyName))
                settings.CompanyName = updateDto.CompanyName;

            if (updateDto.Address != null)
                settings.Address = updateDto.Address;

            if (updateDto.PhoneNumber != null)
                settings.PhoneNumber = updateDto.PhoneNumber;

            if (updateDto.Email != null)
                settings.Email = updateDto.Email;

            if (updateDto.Industry != null)
                settings.Industry = updateDto.Industry;

            if (updateDto.SPOCInformation != null)
                settings.SPOCInformation = updateDto.SPOCInformation;

            if (updateDto.GSTNumber != null)
                settings.GSTNumber = updateDto.GSTNumber;

            if (updateDto.Website != null)
                settings.Website = updateDto.Website;

            if (updateDto.Currency != null)
                settings.Currency = updateDto.Currency;

            if (updateDto.TimeZone != null)
                settings.TimeZone = updateDto.TimeZone;

            if (updateDto.DateFormat != null)
                settings.DateFormat = updateDto.DateFormat;

            if (updateDto.TimeFormat != null)
                settings.TimeFormat = updateDto.TimeFormat;

            if (updateDto.DefaultPageSize.HasValue)
                settings.DefaultPageSize = updateDto.DefaultPageSize.Value;

            if (updateDto.EnableEmailNotifications.HasValue)
                settings.EnableEmailNotifications = updateDto.EnableEmailNotifications.Value;

            if (updateDto.EnableSMSNotifications.HasValue)
                settings.EnableSMSNotifications = updateDto.EnableSMSNotifications.Value;

            settings.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return MapToDto(settings);
        }

        public async Task<bool> UploadCompanyLogoAsync(IFormFile logoFile, string tenantId)
        {
            if (logoFile == null || logoFile.Length == 0)
            {
                return false;
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(logoFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid file type. Only image files (jpg, jpeg, png, gif, bmp) are allowed.");
            }

            // Validate file size (max 5MB)
            if (logoFile.Length > 5 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds 5MB limit.");
            }

            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Ensure database exists
            await context.Database.EnsureCreatedAsync();

            // Normalize tenant ID to lowercase for case-insensitive comparison
            var normalizedTenantId = tenantId?.ToLower();

            var settings = await context.CompanySettings
                .FirstOrDefaultAsync(s => s.TenantId.ToLower() == normalizedTenantId);

            if (settings == null)
            {
                // Create new settings if none exist
                settings = new CompanySettings
                {
                    TenantId = normalizedTenantId,
                    CompanyName = tenantId // Default company name
                };
                context.CompanySettings.Add(settings);
            }

            // Read file into byte array and convert to Base64
            using var memoryStream = new MemoryStream();
            await logoFile.CopyToAsync(memoryStream);
            var logoBytes = memoryStream.ToArray();
            var base64Logo = Convert.ToBase64String(logoBytes);
            var mimeType = logoFile.ContentType ?? GetMimeTypeFromExtension(fileExtension);
            
            // Store as data URI format: data:image/png;base64,{base64string}
            var logoDataUri = $"data:{mimeType};base64,{base64Logo}";

            // Update logo
            settings.CompanyLogo = logoDataUri;
            settings.CompanyLogoMimeType = mimeType;
            settings.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return true;
        }


        private async Task<CompanySettingsDto?> CreateDefaultSettingsAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Ensure database exists
            await context.Database.EnsureCreatedAsync();

            var normalizedTenantId = tenantId?.ToLower();

            var defaultSettings = new CompanySettings
            {
                TenantId = normalizedTenantId,
                CompanyName = tenantId ?? "Company",
                Currency = "USD",
                TimeZone = "UTC",
                DateFormat = "MM/dd/yyyy",
                TimeFormat = "12h",
                DefaultPageSize = 10,
                EnableEmailNotifications = true,
                EnableSMSNotifications = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.CompanySettings.Add(defaultSettings);
            await context.SaveChangesAsync();

            return MapToDto(defaultSettings);
        }

        private static CompanySettingsDto MapToDto(CompanySettings settings)
        {
            return new CompanySettingsDto
            {
                Id = settings.Id,
                TenantId = settings.TenantId,
                CompanyLogo = settings.CompanyLogo, // Base64 string (data URI format)
                CompanyLogoMimeType = settings.CompanyLogoMimeType,
                CompanyName = settings.CompanyName,
                Address = settings.Address,
                PhoneNumber = settings.PhoneNumber,
                Email = settings.Email,
                Industry = settings.Industry,
                SPOCInformation = settings.SPOCInformation,
                GSTNumber = settings.GSTNumber,
                Website = settings.Website,
                Currency = settings.Currency,
                TimeZone = settings.TimeZone,
                DateFormat = settings.DateFormat,
                TimeFormat = settings.TimeFormat,
                DefaultPageSize = settings.DefaultPageSize,
                EnableEmailNotifications = settings.EnableEmailNotifications,
                EnableSMSNotifications = settings.EnableSMSNotifications,
                CreatedAt = settings.CreatedAt,
                UpdatedAt = settings.UpdatedAt
            };
        }

        private static string GetMimeTypeFromExtension(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "image/png"
            };
        }
    }
}

