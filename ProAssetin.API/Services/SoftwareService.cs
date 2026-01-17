using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class SoftwareService : ISoftwareService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public SoftwareService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<SoftwareDto> Software, int TotalCount)> GetSoftwareAsync(SoftwareQueryDto query, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var softwareQuery = context.Software
                .Where(s => s.TenantId.ToLower() == normalizedTenantId)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                softwareQuery = softwareQuery.Where(s =>
                    s.SoftwareName.Contains(query.SearchTerm) ||
                    s.Version!.Contains(query.SearchTerm) ||
                    s.LicenseKey!.Contains(query.SearchTerm) ||
                    s.Description!.Contains(query.SearchTerm) ||
                    s.Category!.Contains(query.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                softwareQuery = softwareQuery.Where(s => s.Status == query.Status);
            }

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                softwareQuery = softwareQuery.Where(s => s.Category == query.Category);
            }

            if (!string.IsNullOrWhiteSpace(query.LicenseType))
            {
                softwareQuery = softwareQuery.Where(s => s.LicenseType == query.LicenseType);
            }

            if (query.VendorId.HasValue)
            {
                softwareQuery = softwareQuery.Where(s => s.VendorId == query.VendorId.Value);
            }

            // Get total count before pagination
            var totalCount = await softwareQuery.CountAsync();

            // Apply sorting
            softwareQuery = query.SortBy?.ToLower() switch
            {
                "name" => query.SortDescending ? softwareQuery.OrderByDescending(s => s.SoftwareName) : softwareQuery.OrderBy(s => s.SoftwareName),
                "version" => query.SortDescending ? softwareQuery.OrderByDescending(s => s.Version) : softwareQuery.OrderBy(s => s.Version),
                "category" => query.SortDescending ? softwareQuery.OrderByDescending(s => s.Category) : softwareQuery.OrderBy(s => s.Category),
                "status" => query.SortDescending ? softwareQuery.OrderByDescending(s => s.Status) : softwareQuery.OrderBy(s => s.Status),
                "expirydate" => query.SortDescending ? softwareQuery.OrderByDescending(s => s.LicenseExpiryDate) : softwareQuery.OrderBy(s => s.LicenseExpiryDate),
                "purchasedate" => query.SortDescending ? softwareQuery.OrderByDescending(s => s.PurchaseDate) : softwareQuery.OrderBy(s => s.PurchaseDate),
                _ => query.SortDescending ? softwareQuery.OrderByDescending(s => s.SoftwareName) : softwareQuery.OrderBy(s => s.SoftwareName)
            };

            // Apply pagination
            var software = await softwareQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            // Get unique user IDs and vendor IDs
            var userIds = software
                .Where(s => !string.IsNullOrEmpty(s.PurchasedByUserId))
                .Select(s => s.PurchasedByUserId!)
                .Distinct()
                .ToList();

            var vendorIds = software
                .Where(s => s.VendorId.HasValue)
                .Select(s => s.VendorId!.Value)
                .Distinct()
                .ToList();

            // Fetch user names from master database
            var users = await _masterContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FirstName, u.LastName })
                .ToListAsync();

            var userNamesDict = users.ToDictionary(
                u => u.Id,
                u => $"{u.FirstName} {u.LastName}"
            );

            // Fetch vendor names from tenant database
            var vendors = await context.Vendors
                .Where(v => vendorIds.Contains(v.Id))
                .Select(v => new { v.Id, v.VendorName })
                .ToListAsync();

            var vendorNamesDict = vendors.ToDictionary(
                v => v.Id,
                v => v.VendorName
            );

            var softwareDtos = software.Select(s => MapToDto(s, userNamesDict, vendorNamesDict));

            return (softwareDtos, totalCount);
        }

        public async Task<SoftwareDto?> GetSoftwareByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var software = await context.Software
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId.ToLower() == normalizedTenantId);

            if (software == null) return null;

            // Fetch user name from master database if assigned
            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(software.PurchasedByUserId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == software.PurchasedByUserId)
                    .Select(u => new { u.Id, u.FirstName, u.LastName })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    userNamesDict = new Dictionary<string, string>
                    {
                        { user.Id, $"{user.FirstName} {user.LastName}" }
                    };
                }
            }

            // Fetch vendor name from tenant database if assigned
            Dictionary<int, string>? vendorNamesDict = null;
            if (software.VendorId.HasValue)
            {
                var vendor = await context.Vendors
                    .Where(v => v.Id == software.VendorId.Value)
                    .Select(v => new { v.Id, v.VendorName })
                    .FirstOrDefaultAsync();

                if (vendor != null)
                {
                    vendorNamesDict = new Dictionary<int, string>
                    {
                        { vendor.Id, vendor.VendorName }
                    };
                }
            }

            return MapToDto(software, userNamesDict, vendorNamesDict);
        }

        public async Task<SoftwareDto> CreateSoftwareAsync(CreateSoftwareDto createDto, string tenantId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            // Get vendor name if vendor ID is provided
            string? vendorName = null;
            if (createDto.VendorId.HasValue)
            {
                var vendor = await context.Vendors
                    .Where(v => v.Id == createDto.VendorId.Value)
                    .Select(v => v.VendorName)
                    .FirstOrDefaultAsync();
                vendorName = vendor;
            }

            // Calculate available licenses
            var usedLicenses = createDto.UsedLicenses ?? 0;
            var totalLicenses = createDto.TotalLicenses ?? 0;
            int? availableLicenses = totalLicenses > 0 ? totalLicenses - usedLicenses : null;

            var software = new Software
            {
                SoftwareName = createDto.SoftwareName,
                Version = createDto.Version,
                LicenseType = createDto.LicenseType,
                LicenseKey = createDto.LicenseKey,
                VendorId = createDto.VendorId,
                VendorName = vendorName,
                PurchasePrice = createDto.PurchasePrice,
                PurchaseDate = createDto.PurchaseDate,
                LicenseExpiryDate = createDto.LicenseExpiryDate,
                TotalLicenses = createDto.TotalLicenses,
                UsedLicenses = createDto.UsedLicenses,
                AvailableLicenses = availableLicenses,
                Description = createDto.Description,
                InstallationPath = createDto.InstallationPath,
                Category = createDto.Category,
                Status = createDto.Status,
                TenantId = normalizedTenantId,
                PurchasedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.Software.Add(software);
            await context.SaveChangesAsync();

            // Fetch user name
            var user = await _masterContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.Id, u.FirstName, u.LastName })
                .FirstOrDefaultAsync();

            var userNamesDict = user != null ? new Dictionary<string, string> { { user.Id, $"{user.FirstName} {user.LastName}" } } : null;
            var vendorNamesDict = vendorName != null && createDto.VendorId.HasValue 
                ? new Dictionary<int, string> { { createDto.VendorId.Value, vendorName } } 
                : null;

            return MapToDto(software, userNamesDict, vendorNamesDict);
        }

        public async Task<SoftwareDto?> UpdateSoftwareAsync(int id, UpdateSoftwareDto updateDto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var software = await context.Software
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId.ToLower() == normalizedTenantId);

            if (software == null) return null;

            // Update fields
            if (!string.IsNullOrWhiteSpace(updateDto.SoftwareName))
                software.SoftwareName = updateDto.SoftwareName;

            if (updateDto.Version != null)
                software.Version = updateDto.Version;

            if (updateDto.LicenseType != null)
                software.LicenseType = updateDto.LicenseType;

            if (updateDto.LicenseKey != null)
                software.LicenseKey = updateDto.LicenseKey;

            if (updateDto.VendorId.HasValue)
            {
                software.VendorId = updateDto.VendorId.Value;
                // Update vendor name
                var vendor = await context.Vendors
                    .Where(v => v.Id == updateDto.VendorId.Value)
                    .Select(v => v.VendorName)
                    .FirstOrDefaultAsync();
                software.VendorName = vendor;
            }

            if (updateDto.PurchasePrice.HasValue)
                software.PurchasePrice = updateDto.PurchasePrice;

            if (updateDto.PurchaseDate.HasValue)
                software.PurchaseDate = updateDto.PurchaseDate;

            if (updateDto.LicenseExpiryDate.HasValue)
                software.LicenseExpiryDate = updateDto.LicenseExpiryDate;

            if (updateDto.TotalLicenses.HasValue)
                software.TotalLicenses = updateDto.TotalLicenses;

            if (updateDto.UsedLicenses.HasValue)
                software.UsedLicenses = updateDto.UsedLicenses;

            // Recalculate available licenses
            if (updateDto.TotalLicenses.HasValue || updateDto.UsedLicenses.HasValue)
            {
                var total = software.TotalLicenses ?? 0;
                var used = software.UsedLicenses ?? 0;
                software.AvailableLicenses = total > 0 ? (int?)(total - used) : null;
            }

            if (updateDto.Description != null)
                software.Description = updateDto.Description;

            if (updateDto.InstallationPath != null)
                software.InstallationPath = updateDto.InstallationPath;

            if (updateDto.Category != null)
                software.Category = updateDto.Category;

            if (!string.IsNullOrWhiteSpace(updateDto.Status))
                software.Status = updateDto.Status;

            software.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            // Fetch user name
            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(software.PurchasedByUserId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == software.PurchasedByUserId)
                    .Select(u => new { u.Id, u.FirstName, u.LastName })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    userNamesDict = new Dictionary<string, string>
                    {
                        { user.Id, $"{user.FirstName} {user.LastName}" }
                    };
                }
            }

            // Fetch vendor name
            Dictionary<int, string>? vendorNamesDict = null;
            if (software.VendorId.HasValue)
            {
                var vendor = await context.Vendors
                    .Where(v => v.Id == software.VendorId.Value)
                    .Select(v => new { v.Id, v.VendorName })
                    .FirstOrDefaultAsync();

                if (vendor != null)
                {
                    vendorNamesDict = new Dictionary<int, string>
                    {
                        { vendor.Id, vendor.VendorName }
                    };
                }
            }

            return MapToDto(software, userNamesDict, vendorNamesDict);
        }

        public async Task<bool> DeleteSoftwareAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var software = await context.Software
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId.ToLower() == normalizedTenantId);

            if (software == null) return false;

            context.Software.Remove(software);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var categories = await context.Software
                .Where(s => s.TenantId.ToLower() == normalizedTenantId && !string.IsNullOrEmpty(s.Category))
                .Select(s => s.Category!)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return categories;
        }

        public Task<IEnumerable<string>> GetLicenseTypesAsync()
        {
            var licenseTypes = new[]
            {
                "Perpetual",
                "Subscription",
                "OEM",
                "Open Source",
                "Freeware",
                "Trial",
                "Volume License",
                "Academic"
            };

            return Task.FromResult<IEnumerable<string>>(licenseTypes);
        }

        public Task<IEnumerable<string>> GetStatusesAsync()
        {
            var statuses = new[]
            {
                "Active",
                "Expired",
                "Inactive",
                "Renewal Pending",
                "Trial",
                "Suspended"
            };

            return Task.FromResult<IEnumerable<string>>(statuses);
        }

        private static SoftwareDto MapToDto(Software software, Dictionary<string, string>? userNamesDict = null, Dictionary<int, string>? vendorNamesDict = null)
        {
            string? purchasedByUserName = null;
            if (!string.IsNullOrEmpty(software.PurchasedByUserId) && userNamesDict != null)
            {
                userNamesDict.TryGetValue(software.PurchasedByUserId, out purchasedByUserName);
            }

            string? vendorName = software.VendorName;
            if (software.VendorId.HasValue && vendorNamesDict != null && string.IsNullOrEmpty(vendorName))
            {
                vendorNamesDict.TryGetValue(software.VendorId.Value, out vendorName);
            }

            return new SoftwareDto
            {
                Id = software.Id,
                SoftwareName = software.SoftwareName,
                Version = software.Version,
                LicenseType = software.LicenseType,
                LicenseKey = software.LicenseKey,
                VendorId = software.VendorId,
                VendorName = vendorName,
                PurchasePrice = software.PurchasePrice,
                PurchaseDate = software.PurchaseDate,
                LicenseExpiryDate = software.LicenseExpiryDate,
                TotalLicenses = software.TotalLicenses,
                UsedLicenses = software.UsedLicenses,
                AvailableLicenses = software.AvailableLicenses,
                Description = software.Description,
                InstallationPath = software.InstallationPath,
                Category = software.Category,
                Status = software.Status,
                PurchasedByUserId = software.PurchasedByUserId,
                PurchasedByUserName = purchasedByUserName,
                CreatedAt = software.CreatedAt,
                UpdatedAt = software.UpdatedAt
            };
        }
    }
}

