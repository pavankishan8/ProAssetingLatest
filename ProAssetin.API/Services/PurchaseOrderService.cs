using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public PurchaseOrderService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<PurchaseOrderDto> PurchaseOrders, int TotalCount)> GetPurchaseOrdersAsync(PurchaseOrderQueryDto query, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var purchaseOrdersQuery = context.PurchaseOrders
                .Where(po => po.TenantId.ToLower() == normalizedTenantId)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                purchaseOrdersQuery = purchaseOrdersQuery.Where(po =>
                    po.PONumber.Contains(query.SearchTerm) ||
                    po.VendorName!.Contains(query.SearchTerm) ||
                    po.Description!.Contains(query.SearchTerm));
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                purchaseOrdersQuery = purchaseOrdersQuery.Where(po => po.Status == query.Status);
            }

            if (query.VendorId.HasValue)
            {
                purchaseOrdersQuery = purchaseOrdersQuery.Where(po => po.VendorId == query.VendorId.Value);
            }

            // Get total count before pagination
            var totalCount = await purchaseOrdersQuery.CountAsync();

            // Apply sorting
            purchaseOrdersQuery = query.SortBy?.ToLower() switch
            {
                "ponumber" => query.SortDescending ? purchaseOrdersQuery.OrderByDescending(po => po.PONumber) : purchaseOrdersQuery.OrderBy(po => po.PONumber),
                "podate" => query.SortDescending ? purchaseOrdersQuery.OrderByDescending(po => po.PODate) : purchaseOrdersQuery.OrderBy(po => po.PODate),
                "totalamount" => query.SortDescending ? purchaseOrdersQuery.OrderByDescending(po => po.TotalAmount) : purchaseOrdersQuery.OrderBy(po => po.TotalAmount),
                "status" => query.SortDescending ? purchaseOrdersQuery.OrderByDescending(po => po.Status) : purchaseOrdersQuery.OrderBy(po => po.Status),
                _ => query.SortDescending ? purchaseOrdersQuery.OrderByDescending(po => po.PODate) : purchaseOrdersQuery.OrderByDescending(po => po.PODate)
            };

            // Apply pagination
            var purchaseOrders = await purchaseOrdersQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            // Get unique user IDs and vendor IDs
            var userIds = purchaseOrders
                .Where(po => !string.IsNullOrEmpty(po.CreatedByUserId) || !string.IsNullOrEmpty(po.ApprovedByUserId))
                .SelectMany(po => new[] { po.CreatedByUserId, po.ApprovedByUserId })
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            var vendorIds = purchaseOrders
                .Where(po => po.VendorId.HasValue)
                .Select(po => po.VendorId!.Value)
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

            var purchaseOrderDtos = purchaseOrders.Select(po => MapToDto(po, userNamesDict, vendorNamesDict));

            return (purchaseOrderDtos, totalCount);
        }

        public async Task<PurchaseOrderDto?> GetPurchaseOrderByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var purchaseOrder = await context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.Id == id && po.TenantId.ToLower() == normalizedTenantId);

            if (purchaseOrder == null) return null;

            // Fetch user names from master database
            Dictionary<string, string>? userNamesDict = null;
            var userIds = new List<string>();
            if (!string.IsNullOrEmpty(purchaseOrder.CreatedByUserId)) userIds.Add(purchaseOrder.CreatedByUserId);
            if (!string.IsNullOrEmpty(purchaseOrder.ApprovedByUserId)) userIds.Add(purchaseOrder.ApprovedByUserId);

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

            // Fetch vendor name from tenant database
            Dictionary<int, string>? vendorNamesDict = null;
            if (purchaseOrder.VendorId.HasValue)
            {
                var vendor = await context.Vendors
                    .Where(v => v.Id == purchaseOrder.VendorId.Value)
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

            return MapToDto(purchaseOrder, userNamesDict, vendorNamesDict);
        }

        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto createDto, string tenantId, string userId)
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

            var purchaseOrder = new PurchaseOrder
            {
                PONumber = createDto.PONumber,
                VendorId = createDto.VendorId,
                VendorName = vendorName,
                TotalAmount = createDto.TotalAmount,
                PODate = createDto.PODate,
                ExpectedDeliveryDate = createDto.ExpectedDeliveryDate,
                Status = createDto.Status,
                Description = createDto.Description,
                TenantId = normalizedTenantId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.PurchaseOrders.Add(purchaseOrder);
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

            return MapToDto(purchaseOrder, userNamesDict, vendorNamesDict);
        }

        public async Task<PurchaseOrderDto?> UpdatePurchaseOrderAsync(int id, UpdatePurchaseOrderDto updateDto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var purchaseOrder = await context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.Id == id && po.TenantId.ToLower() == normalizedTenantId);

            if (purchaseOrder == null) return null;

            // Update fields
            if (!string.IsNullOrWhiteSpace(updateDto.PONumber))
                purchaseOrder.PONumber = updateDto.PONumber;

            if (updateDto.VendorId.HasValue)
            {
                purchaseOrder.VendorId = updateDto.VendorId.Value;
                // Update vendor name
                var vendor = await context.Vendors
                    .Where(v => v.Id == updateDto.VendorId.Value)
                    .Select(v => v.VendorName)
                    .FirstOrDefaultAsync();
                purchaseOrder.VendorName = vendor;
            }

            if (updateDto.TotalAmount.HasValue)
                purchaseOrder.TotalAmount = updateDto.TotalAmount.Value;

            if (updateDto.PODate.HasValue)
                purchaseOrder.PODate = updateDto.PODate.Value;

            if (updateDto.ExpectedDeliveryDate.HasValue)
                purchaseOrder.ExpectedDeliveryDate = updateDto.ExpectedDeliveryDate;

            if (!string.IsNullOrWhiteSpace(updateDto.Status))
                purchaseOrder.Status = updateDto.Status;

            if (updateDto.Description != null)
                purchaseOrder.Description = updateDto.Description;

            if (!string.IsNullOrWhiteSpace(updateDto.ApprovedByUserId))
                purchaseOrder.ApprovedByUserId = updateDto.ApprovedByUserId;

            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            // Fetch user names
            Dictionary<string, string>? userNamesDict = null;
            var userIds = new List<string>();
            if (!string.IsNullOrEmpty(purchaseOrder.CreatedByUserId)) userIds.Add(purchaseOrder.CreatedByUserId);
            if (!string.IsNullOrEmpty(purchaseOrder.ApprovedByUserId)) userIds.Add(purchaseOrder.ApprovedByUserId);

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

            // Fetch vendor name
            Dictionary<int, string>? vendorNamesDict = null;
            if (purchaseOrder.VendorId.HasValue)
            {
                var vendor = await context.Vendors
                    .Where(v => v.Id == purchaseOrder.VendorId.Value)
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

            return MapToDto(purchaseOrder, userNamesDict, vendorNamesDict);
        }

        public async Task<bool> DeletePurchaseOrderAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var purchaseOrder = await context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.Id == id && po.TenantId.ToLower() == normalizedTenantId);

            if (purchaseOrder == null) return false;

            context.PurchaseOrders.Remove(purchaseOrder);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<PurchaseOrderDto?> ApprovePurchaseOrderAsync(int id, string tenantId, string approvedByUserId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var purchaseOrder = await context.PurchaseOrders
                .FirstOrDefaultAsync(po => po.Id == id && po.TenantId.ToLower() == normalizedTenantId);

            if (purchaseOrder == null) return null;

            purchaseOrder.Status = "Approved";
            purchaseOrder.ApprovedByUserId = approvedByUserId;
            purchaseOrder.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            // Fetch user names
            var userIds = new List<string>();
            if (!string.IsNullOrEmpty(purchaseOrder.CreatedByUserId)) userIds.Add(purchaseOrder.CreatedByUserId);
            if (!string.IsNullOrEmpty(purchaseOrder.ApprovedByUserId)) userIds.Add(purchaseOrder.ApprovedByUserId);

            var users = await _masterContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FirstName, u.LastName })
                .ToListAsync();

            var userNamesDict = users.ToDictionary(
                u => u.Id,
                u => $"{u.FirstName} {u.LastName}"
            );

            // Fetch vendor name
            Dictionary<int, string>? vendorNamesDict = null;
            if (purchaseOrder.VendorId.HasValue)
            {
                var vendor = await context.Vendors
                    .Where(v => v.Id == purchaseOrder.VendorId.Value)
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

            return MapToDto(purchaseOrder, userNamesDict, vendorNamesDict);
        }

        private static PurchaseOrderDto MapToDto(PurchaseOrder purchaseOrder, Dictionary<string, string>? userNamesDict = null, Dictionary<int, string>? vendorNamesDict = null)
        {
            string? createdByUserName = null;
            if (!string.IsNullOrEmpty(purchaseOrder.CreatedByUserId) && userNamesDict != null)
            {
                userNamesDict.TryGetValue(purchaseOrder.CreatedByUserId, out createdByUserName);
            }

            string? approvedByUserName = null;
            if (!string.IsNullOrEmpty(purchaseOrder.ApprovedByUserId) && userNamesDict != null)
            {
                userNamesDict.TryGetValue(purchaseOrder.ApprovedByUserId, out approvedByUserName);
            }

            string? vendorName = purchaseOrder.VendorName;
            if (purchaseOrder.VendorId.HasValue && vendorNamesDict != null && string.IsNullOrEmpty(vendorName))
            {
                vendorNamesDict.TryGetValue(purchaseOrder.VendorId.Value, out vendorName);
            }

            return new PurchaseOrderDto
            {
                Id = purchaseOrder.Id,
                PONumber = purchaseOrder.PONumber,
                VendorId = purchaseOrder.VendorId,
                VendorName = vendorName,
                TotalAmount = purchaseOrder.TotalAmount,
                PODate = purchaseOrder.PODate,
                ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
                Status = purchaseOrder.Status,
                Description = purchaseOrder.Description,
                CreatedByUserId = purchaseOrder.CreatedByUserId,
                CreatedByUserName = createdByUserName,
                ApprovedByUserId = purchaseOrder.ApprovedByUserId,
                ApprovedByUserName = approvedByUserName,
                CreatedAt = purchaseOrder.CreatedAt,
                UpdatedAt = purchaseOrder.UpdatedAt
            };
        }
    }
}

