using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class EWasteService : IEWasteService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public EWasteService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<EWasteDisposalDto> Items, int TotalCount)> GetDisposalsAsync(EWasteDisposalQueryDto query, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLowerInvariant();

            var q = context.EWasteDisposals
                .Include(e => e.Asset)
                .Where(e => e.TenantId.ToLower() == normalizedTenantId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm;
                q = q.Where(e =>
                    e.DisposalReference.Contains(term) ||
                    e.ItemDescription.Contains(term) ||
                    (e.Category != null && e.Category.Contains(term)) ||
                    (e.RecyclerName != null && e.RecyclerName.Contains(term)) ||
                    (e.CertificateReference != null && e.CertificateReference.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(query.Status))
                q = q.Where(e => e.Status == query.Status);

            if (query.DisposalDateFrom.HasValue)
                q = q.Where(e => e.DisposalDate >= query.DisposalDateFrom.Value);

            if (query.DisposalDateTo.HasValue)
                q = q.Where(e => e.DisposalDate <= query.DisposalDateTo.Value);

            var totalCount = await q.CountAsync();

            q = query.SortBy?.ToLower() switch
            {
                "disposalreference" => query.SortDescending ? q.OrderByDescending(e => e.DisposalReference) : q.OrderBy(e => e.DisposalReference),
                "disposaldate" => query.SortDescending ? q.OrderByDescending(e => e.DisposalDate) : q.OrderBy(e => e.DisposalDate),
                "pickupdate" => query.SortDescending ? q.OrderByDescending(e => e.PickupDate) : q.OrderBy(e => e.PickupDate),
                "status" => query.SortDescending ? q.OrderByDescending(e => e.Status) : q.OrderBy(e => e.Status),
                "recyclername" => query.SortDescending ? q.OrderByDescending(e => e.RecyclerName) : q.OrderBy(e => e.RecyclerName),
                _ => query.SortDescending ? q.OrderByDescending(e => e.CreatedAt) : q.OrderByDescending(e => e.CreatedAt)
            };

            var rows = await q
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var userIds = rows.Where(e => !string.IsNullOrEmpty(e.CreatedByUserId)).Select(e => e.CreatedByUserId!).Distinct().ToList();
            var userNames = await ResolveUserNamesAsync(userIds);

            var dtos = rows.Select(e => MapToDto(e, userNames));
            return (dtos, totalCount);
        }

        public async Task<EWasteDisposalDto?> GetDisposalByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLower();

            var row = await context.EWasteDisposals
                .Include(e => e.Asset)
                .FirstOrDefaultAsync(e => e.Id == id && e.TenantId.ToLower() == normalizedTenantId);

            if (row == null) return null;

            var userNames = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, userNames);
        }

        public async Task<EWasteDisposalDto> CreateDisposalAsync(CreateEWasteDisposalDto dto, string tenantId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLowerInvariant()!;

            await EnsureValidAssetAsync(context, dto.AssetId, normalizedTenantId);

            var qty = dto.Quantity < 1 ? 1 : dto.Quantity;
            var row = new EWasteDisposal
            {
                DisposalReference = dto.DisposalReference,
                AssetId = dto.AssetId,
                ItemDescription = dto.ItemDescription,
                Category = dto.Category,
                Quantity = qty,
                EstimatedWeightKg = dto.EstimatedWeightKg,
                RecyclerName = dto.RecyclerName,
                PickupDate = dto.PickupDate,
                DisposalDate = dto.DisposalDate,
                CertificateReference = dto.CertificateReference,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Scheduled" : dto.Status,
                Notes = dto.Notes,
                TenantId = normalizedTenantId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.EWasteDisposals.Add(row);
            await context.SaveChangesAsync();

            await context.Entry(row).Reference(e => e.Asset).LoadAsync();
            var userNames = await ResolveUserNamesAsync(new List<string> { userId });
            return MapToDto(row, userNames);
        }

        public async Task<EWasteDisposalDto?> UpdateDisposalAsync(int id, UpdateEWasteDisposalDto dto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLower();

            var row = await context.EWasteDisposals
                .FirstOrDefaultAsync(e => e.Id == id && e.TenantId.ToLower() == normalizedTenantId);

            if (row == null) return null;

            if (dto.AssetId.HasValue)
            {
                await EnsureValidAssetAsync(context, dto.AssetId, normalizedTenantId!);
                row.AssetId = dto.AssetId;
            }

            if (!string.IsNullOrWhiteSpace(dto.DisposalReference))
                row.DisposalReference = dto.DisposalReference;

            if (dto.ItemDescription != null)
                row.ItemDescription = dto.ItemDescription;

            if (dto.Category != null)
                row.Category = dto.Category;

            if (dto.Quantity.HasValue)
                row.Quantity = dto.Quantity.Value < 1 ? 1 : dto.Quantity.Value;

            if (dto.EstimatedWeightKg.HasValue)
                row.EstimatedWeightKg = dto.EstimatedWeightKg;

            if (dto.RecyclerName != null)
                row.RecyclerName = dto.RecyclerName;

            if (dto.PickupDate.HasValue)
                row.PickupDate = dto.PickupDate;

            if (dto.DisposalDate.HasValue)
                row.DisposalDate = dto.DisposalDate;

            if (dto.CertificateReference != null)
                row.CertificateReference = dto.CertificateReference;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                row.Status = dto.Status;

            if (dto.Notes != null)
                row.Notes = dto.Notes;

            row.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            await context.Entry(row).Reference(e => e.Asset).LoadAsync();
            var userNames = await ResolveUserNamesAsync(
                string.IsNullOrEmpty(row.CreatedByUserId) ? new List<string>() : new List<string> { row.CreatedByUserId! });

            return MapToDto(row, userNames);
        }

        public async Task<bool> DeleteDisposalAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);
            var normalizedTenantId = tenantId?.ToLower();

            var row = await context.EWasteDisposals
                .FirstOrDefaultAsync(e => e.Id == id && e.TenantId.ToLower() == normalizedTenantId);

            if (row == null) return false;

            context.EWasteDisposals.Remove(row);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> GetStatusesAsync()
        {
            return await Task.FromResult(new[] { "Scheduled", "Collected", "Disposed", "Cancelled" });
        }

        private async Task EnsureValidAssetAsync(TenantDbContext context, int? assetId, string normalizedTenantId)
        {
            if (!assetId.HasValue) return;

            var exists = await context.Assets.AnyAsync(a =>
                a.Id == assetId.Value && a.TenantId.ToLower() == normalizedTenantId.ToLower());

            if (!exists)
                throw new InvalidOperationException("Asset not found for this tenant.");
        }

        private async Task<Dictionary<string, string>> ResolveUserNamesAsync(List<string> userIds)
        {
            if (!userIds.Any()) return new Dictionary<string, string>();

            var users = await _masterContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.FirstName, u.LastName })
                .ToListAsync();

            return users.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");
        }

        private static EWasteDisposalDto MapToDto(EWasteDisposal e, Dictionary<string, string> userNames)
        {
            string? createdName = null;
            if (!string.IsNullOrEmpty(e.CreatedByUserId))
                userNames.TryGetValue(e.CreatedByUserId, out createdName);

            return new EWasteDisposalDto
            {
                Id = e.Id,
                DisposalReference = e.DisposalReference,
                AssetId = e.AssetId,
                AssetTag = e.Asset?.AssetId,
                ItemDescription = e.ItemDescription,
                Category = e.Category,
                Quantity = e.Quantity,
                EstimatedWeightKg = e.EstimatedWeightKg,
                RecyclerName = e.RecyclerName,
                PickupDate = e.PickupDate,
                DisposalDate = e.DisposalDate,
                CertificateReference = e.CertificateReference,
                Status = e.Status,
                Notes = e.Notes,
                CreatedByUserId = e.CreatedByUserId,
                CreatedByUserName = createdName,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            };
        }
    }
}
