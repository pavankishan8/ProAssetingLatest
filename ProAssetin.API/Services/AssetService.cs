using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class AssetService : IAssetService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public AssetService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<AssetDto> Assets, int TotalCount)> GetAssetsAsync(AssetQueryDto query, string tenantId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext(tenantId);

                // Normalize tenant ID to lowercase for comparison (TenantResolver returns lowercase)
                var normalizedTenantId = tenantId?.ToLowerInvariant() ?? string.Empty;

                // Query assets - since TenantId in DB is "infosys" (lowercase) and tenantId from JWT is also lowercase
                // Use direct comparison first, fallback to case-insensitive if needed
                var assetsQuery = context.Assets
                    .Where(a => a.TenantId != null && a.TenantId.ToLower() == normalizedTenantId)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                {
                    assetsQuery = assetsQuery.Where(a =>
                        a.Name.Contains(query.SearchTerm) ||
                        a.AssetId.Contains(query.SearchTerm) ||
                        a.SerialNumber!.Contains(query.SearchTerm) ||
                        a.Category.Contains(query.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(query.Category))
                {
                    assetsQuery = assetsQuery.Where(a => a.Category == query.Category);
                }

                if (!string.IsNullOrWhiteSpace(query.Status))
                {
                    assetsQuery = assetsQuery.Where(a => a.Status == query.Status);
                }

                if (!string.IsNullOrWhiteSpace(query.Location))
                {
                    assetsQuery = assetsQuery.Where(a => a.Location == query.Location);
                }

                // Get total count before pagination
                int totalCount;
                try
                {
                    totalCount = await assetsQuery.CountAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error counting assets: {ex.Message}", ex);
                }

                // Apply sorting
                assetsQuery = query.SortBy?.ToLower() switch
                {
                    "name" => query.SortDescending ? assetsQuery.OrderByDescending(a => a.Name) : assetsQuery.OrderBy(a => a.Name),
                    "category" => query.SortDescending ? assetsQuery.OrderByDescending(a => a.Category) : assetsQuery.OrderBy(a => a.Category),
                    "status" => query.SortDescending ? assetsQuery.OrderByDescending(a => a.Status) : assetsQuery.OrderBy(a => a.Status),
                    "purchasedate" => query.SortDescending ? assetsQuery.OrderByDescending(a => a.PurchaseDate) : assetsQuery.OrderBy(a => a.PurchaseDate),
                    _ => query.SortDescending ? assetsQuery.OrderByDescending(a => a.Name) : assetsQuery.OrderBy(a => a.Name)
                };

                // Apply pagination
                List<Asset> assets;
                try
                {
                    assets = await assetsQuery
                        .Skip((query.PageNumber - 1) * query.PageSize)
                        .Take(query.PageSize)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving paginated assets: {ex.Message}", ex);
                }

                // Get unique user IDs from assets
                var userIds = assets
                    .Where(a => !string.IsNullOrEmpty(a.AssignedToUserId))
                    .Select(a => a.AssignedToUserId!)
                    .Distinct()
                    .ToList();

                // Fetch user names from master database
                Dictionary<string, string> userNamesDict = new();
                try
                {
                    if (userIds.Any())
                    {
                        var users = await _masterContext.Users
                            .Where(u => userIds.Contains(u.Id))
                            .Select(u => new { u.Id, u.FirstName, u.LastName })
                            .ToListAsync();

                        userNamesDict = users.ToDictionary(
                            u => u.Id,
                            u => $"{u.FirstName} {u.LastName}"
                        );
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error fetching user names: {ex.Message}", ex);
                }

                // Get unique vendor IDs from assets
                var vendorIds = assets
                    .Where(a => a.VendorId.HasValue)
                    .Select(a => a.VendorId!.Value)
                    .Distinct()
                    .ToList();

                // Fetch vendor names from tenant database
                Dictionary<int, string> vendorNamesDict = new();
                try
                {
                    if (vendorIds.Any())
                    {
                        var vendors = await context.Vendors
                            .Where(v => vendorIds.Contains(v.Id))
                            .Select(v => new { v.Id, v.VendorName })
                            .ToListAsync();

                        vendorNamesDict = vendors.ToDictionary(
                            v => v.Id,
                            v => v.VendorName
                        );
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error fetching vendor names: {ex.Message}", ex);
                }

                // Map to DTOs
                IEnumerable<AssetDto> assetDtos;
                try
                {
                    assetDtos = assets.Select(a => MapToDto(a, userNamesDict, vendorNamesDict));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error mapping assets to DTOs: {ex.Message}", ex);
                }

                return (assetDtos, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetAssetsAsync for tenant {tenantId}: {ex.Message}", ex);
            }
        }

        public async Task<AssetDto?> GetAssetByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Normalize tenant ID to lowercase for case-insensitive comparison
            var normalizedTenantId = tenantId?.ToLower();

            var asset = await context.Assets
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId.ToLower() == normalizedTenantId);

            if (asset == null) return null;

            // Fetch user name from master database if assigned
            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(asset.AssignedToUserId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == asset.AssignedToUserId)
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
            if (asset.VendorId.HasValue)
            {
                var vendor = await context.Vendors
                    .Where(v => v.Id == asset.VendorId.Value)
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

            return MapToDto(asset, userNamesDict, vendorNamesDict);
        }

        public async Task<AssetDto> CreateAssetAsync(CreateAssetDto createDto, string tenantId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var asset = new Asset
            {
                AssetId = createDto.AssetId,
                Name = createDto.Name,
                Category = createDto.Category,
                Location = createDto.Location,
                Status = createDto.Status,
                Make = createDto.Make,
                Model = createDto.Model,
                SerialNumber = createDto.SerialNumber,
                PurchasePrice = createDto.PurchasePrice,
                PurchaseDate = createDto.PurchaseDate,
                WarrantyExpiryDate = createDto.WarrantyExpiryDate,
                Description = createDto.Description,
                AssignedToUserId = createDto.AssignedToUserId,
                VendorId = createDto.VendorId,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow
            };

            context.Assets.Add(asset);
            await context.SaveChangesAsync();

            // Fetch user name from master database if assigned
            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(asset.AssignedToUserId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == asset.AssignedToUserId)
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
            if (asset.VendorId.HasValue)
            {
                var vendor = await context.Vendors
                    .Where(v => v.Id == asset.VendorId.Value)
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

            return MapToDto(asset, userNamesDict, vendorNamesDict);
        }

        public async Task<AssetDto?> UpdateAssetAsync(int id, UpdateAssetDto updateDto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Normalize tenant ID to lowercase for case-insensitive comparison
            var normalizedTenantId = tenantId?.ToLower();

            var asset = await context.Assets
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId.ToLower() == normalizedTenantId);

            if (asset == null) return null;

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(updateDto.Name)) asset.Name = updateDto.Name;
            if (!string.IsNullOrWhiteSpace(updateDto.Category)) asset.Category = updateDto.Category;
            if (updateDto.Location != null) asset.Location = updateDto.Location;
            if (!string.IsNullOrWhiteSpace(updateDto.Status)) asset.Status = updateDto.Status;
            if (updateDto.Make != null) asset.Make = updateDto.Make;
            if (updateDto.Model != null) asset.Model = updateDto.Model;
            if (updateDto.SerialNumber != null) asset.SerialNumber = updateDto.SerialNumber;
            if (updateDto.PurchasePrice.HasValue) asset.PurchasePrice = updateDto.PurchasePrice;
            if (updateDto.PurchaseDate.HasValue) asset.PurchaseDate = updateDto.PurchaseDate;
            if (updateDto.WarrantyExpiryDate.HasValue) asset.WarrantyExpiryDate = updateDto.WarrantyExpiryDate;
            if (updateDto.Description != null) asset.Description = updateDto.Description;
            if (updateDto.AssignedToUserId != null) asset.AssignedToUserId = updateDto.AssignedToUserId;
            if (updateDto.VendorId.HasValue) asset.VendorId = updateDto.VendorId;

            asset.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            // Fetch user name from master database if assigned
            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(asset.AssignedToUserId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == asset.AssignedToUserId)
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
            if (asset.VendorId.HasValue)
            {
                var vendor = await context.Vendors
                    .Where(v => v.Id == asset.VendorId.Value)
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

            return MapToDto(asset, userNamesDict, vendorNamesDict);
        }

        public async Task<bool> DeleteAssetAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Normalize tenant ID to lowercase for case-insensitive comparison
            var normalizedTenantId = tenantId?.ToLower();

            var asset = await context.Assets
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId.ToLower() == normalizedTenantId);

            if (asset == null) return false;

            context.Assets.Remove(asset);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            // Normalize tenant ID to lowercase for case-insensitive comparison
            var normalizedTenantId = tenantId?.ToLower();

            return await context.Assets
                .Where(a => a.TenantId.ToLower() == normalizedTenantId && !string.IsNullOrEmpty(a.Category))
                .Select(a => a.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetStatusesAsync()
        {
            return await Task.FromResult(new[] { "In-Stock", "Repair", "Sold", "Damaged", "E-Waste" });
        }

        public async Task<IEnumerable<AssetDto>> ImportAssetsFromExcelAsync(IFormFile file, string tenantId, string userId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var importedAssets = new List<AssetDto>();

            using var context = _dbContextFactory.CreateDbContext(tenantId);
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using (var package = new ExcelPackage(stream))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    throw new InvalidOperationException("Excel file must contain at least one worksheet.");
                }

                var worksheet = package.Workbook.Worksheets[0];

                if (worksheet.Dimension == null)
                {
                    throw new InvalidOperationException("Excel worksheet is empty.");
                }

                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Read header row to determine column positions
                var headerRow = 1;
                var columnMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                
                for (int col = 1; col <= colCount; col++)
                {
                    var headerValue = worksheet.Cells[headerRow, col].Text?.Trim() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(headerValue))
                    {
                        // Normalize header names (remove spaces, handle common variations)
                        var normalizedHeader = headerValue
                            .Replace(" ", "")
                            .Replace("_", "")
                            .Replace("-", "");
                        
                        columnMap[normalizedHeader] = col;
                    }
                }

                // Map column indices with fallback to expected positions
                int GetColumnIndex(string[] possibleNames, int fallbackIndex)
                {
                    foreach (var name in possibleNames)
                    {
                        var normalized = name.Replace(" ", "").Replace("_", "").Replace("-", "");
                        if (columnMap.ContainsKey(normalized))
                            return columnMap[normalized];
                    }
                    return fallbackIndex;
                }

                var assetIdCol = GetColumnIndex(new[] { "AssetId", "Asset ID", "Asset_Id" }, 1);
                var nameCol = GetColumnIndex(new[] { "Name", "AssetName", "Asset Name" }, 2);
                var categoryCol = GetColumnIndex(new[] { "Category" }, 3);
                var statusCol = GetColumnIndex(new[] { "Status" }, 4);
                var locationCol = GetColumnIndex(new[] { "Location" }, 5);
                var makeCol = GetColumnIndex(new[] { "Make", "Manufacturer" }, 6);
                var modelCol = GetColumnIndex(new[] { "Model" }, 7);
                var serialNumberCol = GetColumnIndex(new[] { "SerialNumber", "Serial Number", "Serial_Number" }, 8);
                var purchasePriceCol = GetColumnIndex(new[] { "PurchasePrice", "Purchase Price", "Purchase_Price", "Price" }, 9);
                var purchaseDateCol = GetColumnIndex(new[] { "PurchaseDate", "Purchase Date", "Purchase_Date" }, 10);
                var warrantyExpiryDateCol = GetColumnIndex(new[] { "WarrantyExpiryDate", "Warranty Expiry Date", "Warranty_Expiry_Date", "WarrantyDate" }, 11);
                var descriptionCol = GetColumnIndex(new[] { "Description", "Notes", "Remarks" }, 12);
                var assignedToUserIdCol = GetColumnIndex(new[] { "AssignedToUserId", "Assigned To User Id", "Assigned_To_User_Id", "AssignedTo", "UserID" }, 13);

                // Skip header row (row 1), start from row 2
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        // Read cell values using column mapping
                        var assetId = worksheet.Cells[row, assetIdCol].Text?.Trim();
                        var name = worksheet.Cells[row, nameCol].Text?.Trim();
                        var category = worksheet.Cells[row, categoryCol].Text?.Trim();
                        var status = worksheet.Cells[row, statusCol].Text?.Trim();
                        var location = worksheet.Cells[row, locationCol].Text?.Trim();
                        var make = worksheet.Cells[row, makeCol].Text?.Trim();
                        var model = worksheet.Cells[row, modelCol].Text?.Trim();
                        var serialNumber = worksheet.Cells[row, serialNumberCol].Text?.Trim();
                        var purchasePriceText = worksheet.Cells[row, purchasePriceCol].Text?.Trim();
                        var purchaseDateText = worksheet.Cells[row, purchaseDateCol].Text?.Trim();
                        var warrantyExpiryDateText = worksheet.Cells[row, warrantyExpiryDateCol].Text?.Trim();
                        var description = worksheet.Cells[row, descriptionCol].Text?.Trim();
                        var assignedToUserId = worksheet.Cells[row, assignedToUserIdCol].Text?.Trim();

                        // Skip empty rows
                        if (string.IsNullOrWhiteSpace(assetId) && string.IsNullOrWhiteSpace(name))
                        {
                            continue;
                        }

                        // Validate required fields
                        if (string.IsNullOrWhiteSpace(assetId))
                        {
                            assetId = $"ASSET-{Guid.NewGuid():N}"; // Generate if missing
                        }

                        if (string.IsNullOrWhiteSpace(name))
                        {
                            continue; // Skip rows without name
                        }

                        // Parse dates
                        DateTime? purchaseDate = null;
                        if (!string.IsNullOrWhiteSpace(purchaseDateText))
                        {
                            if (DateTime.TryParse(purchaseDateText, out var parsedPurchaseDate))
                            {
                                purchaseDate = parsedPurchaseDate;
                            }
                            else if (double.TryParse(purchaseDateText, out var oaDate))
                            {
                                purchaseDate = DateTime.FromOADate(oaDate);
                            }
                        }

                        DateTime? warrantyExpiryDate = null;
                        if (!string.IsNullOrWhiteSpace(warrantyExpiryDateText))
                        {
                            if (DateTime.TryParse(warrantyExpiryDateText, out var parsedWarrantyDate))
                            {
                                warrantyExpiryDate = parsedWarrantyDate;
                            }
                            else if (double.TryParse(warrantyExpiryDateText, out var oaDate))
                            {
                                warrantyExpiryDate = DateTime.FromOADate(oaDate);
                            }
                        }

                        // Parse decimal
                        decimal? purchasePrice = null;
                        if (!string.IsNullOrWhiteSpace(purchasePriceText))
                        {
                            if (decimal.TryParse(purchasePriceText, out var parsedPrice))
                            {
                                purchasePrice = parsedPrice;
                            }
                        }

                        // Default status if empty
                        if (string.IsNullOrWhiteSpace(status))
                        {
                            status = "In-Stock";
                        }

                        // Default category if empty
                        if (string.IsNullOrWhiteSpace(category))
                        {
                            category = "Uncategorized";
                        }

                        // Create asset
                        var asset = new Asset
                        {
                            AssetId = assetId,
                            Name = name,
                            Category = category,
                            Location = string.IsNullOrWhiteSpace(location) ? null : location,
                            Status = status,
                            Make = string.IsNullOrWhiteSpace(make) ? null : make,
                            Model = string.IsNullOrWhiteSpace(model) ? null : model,
                            SerialNumber = string.IsNullOrWhiteSpace(serialNumber) ? null : serialNumber,
                            PurchasePrice = purchasePrice,
                            PurchaseDate = purchaseDate,
                            WarrantyExpiryDate = warrantyExpiryDate,
                            Description = string.IsNullOrWhiteSpace(description) ? null : description,
                            AssignedToUserId = string.IsNullOrWhiteSpace(assignedToUserId) ? null : assignedToUserId,
                            TenantId = tenantId,
                            CreatedAt = DateTime.UtcNow
                        };

                        context.Assets.Add(asset);
                        await context.SaveChangesAsync();

                        // Fetch user name from master database if assigned
                        Dictionary<string, string>? userNamesDict = null;
                        if (!string.IsNullOrEmpty(asset.AssignedToUserId))
                        {
                            var user = await _masterContext.Users
                                .Where(u => u.Id == asset.AssignedToUserId)
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

                        importedAssets.Add(MapToDto(asset, userNamesDict, null));
                    }
                    catch (Exception ex)
                    {
                        // Log error for this row but continue processing
                        // In production, you might want to collect errors and return them
                        throw new InvalidOperationException($"Error processing row {row}: {ex.Message}", ex);
                    }
                }
            }

            return importedAssets;
        }

        private static AssetDto MapToDto(Asset asset, Dictionary<string, string>? userNamesDict = null, Dictionary<int, string>? vendorNamesDict = null)
        {
            string? assignedToUserName = null;
            if (!string.IsNullOrEmpty(asset.AssignedToUserId) && userNamesDict != null)
            {
                userNamesDict.TryGetValue(asset.AssignedToUserId, out assignedToUserName);
            }

            string? vendorName = null;
            if (asset.VendorId.HasValue && vendorNamesDict != null)
            {
                vendorNamesDict.TryGetValue(asset.VendorId.Value, out vendorName);
            }

            return new AssetDto
            {
                Id = asset.Id,
                AssetId = asset.AssetId,
                Name = asset.Name,
                Category = asset.Category,
                Location = asset.Location,
                Status = asset.Status,
                Make = asset.Make,
                Model = asset.Model,
                SerialNumber = asset.SerialNumber,
                PurchasePrice = asset.PurchasePrice,
                PurchaseDate = asset.PurchaseDate,
                WarrantyExpiryDate = asset.WarrantyExpiryDate,
                Description = asset.Description,
                AssignedToUserId = asset.AssignedToUserId,
                AssignedToUserName = assignedToUserName,
                VendorId = asset.VendorId,
                VendorName = vendorName,
                CreatedAt = asset.CreatedAt,
                UpdatedAt = asset.UpdatedAt
            };
        }
    }
}

