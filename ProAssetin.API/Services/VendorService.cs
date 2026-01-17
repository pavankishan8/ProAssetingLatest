using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class VendorService : IVendorService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;

        public VendorService(ITenantDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<(IEnumerable<VendorDto> Vendors, int TotalCount)> GetVendorsAsync(VendorQueryDto query, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var vendorsQuery = context.Vendors
                .Where(v => v.TenantId.ToLower() == normalizedTenantId)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                vendorsQuery = vendorsQuery.Where(v =>
                    v.VendorName.Contains(query.SearchTerm) ||
                    v.ContactPerson!.Contains(query.SearchTerm) ||
                    v.Email!.Contains(query.SearchTerm) ||
                    v.PhoneNumber!.Contains(query.SearchTerm) ||
                    v.City!.Contains(query.SearchTerm));
            }

            if (query.IsActive.HasValue)
            {
                vendorsQuery = vendorsQuery.Where(v => v.IsActive == query.IsActive.Value);
            }

            // Get total count before pagination
            var totalCount = await vendorsQuery.CountAsync();

            // Apply sorting
            vendorsQuery = query.SortBy?.ToLower() switch
            {
                "name" => query.SortDescending ? vendorsQuery.OrderByDescending(v => v.VendorName) : vendorsQuery.OrderBy(v => v.VendorName),
                "contactperson" => query.SortDescending ? vendorsQuery.OrderByDescending(v => v.ContactPerson) : vendorsQuery.OrderBy(v => v.ContactPerson),
                "city" => query.SortDescending ? vendorsQuery.OrderByDescending(v => v.City) : vendorsQuery.OrderBy(v => v.City),
                "createdat" => query.SortDescending ? vendorsQuery.OrderByDescending(v => v.CreatedAt) : vendorsQuery.OrderBy(v => v.CreatedAt),
                _ => query.SortDescending ? vendorsQuery.OrderByDescending(v => v.VendorName) : vendorsQuery.OrderBy(v => v.VendorName)
            };

            // Apply pagination
            var vendors = await vendorsQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (vendors.Select(MapToDto), totalCount);
        }

        public async Task<VendorDto?> GetVendorByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var vendor = await context.Vendors
                .FirstOrDefaultAsync(v => v.Id == id && v.TenantId.ToLower() == normalizedTenantId);

            return vendor != null ? MapToDto(vendor) : null;
        }

        public async Task<VendorDto> CreateVendorAsync(CreateVendorDto createDto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var vendor = new Vendor
            {
                VendorName = createDto.VendorName,
                ContactPerson = createDto.ContactPerson,
                Email = createDto.Email,
                PhoneNumber = createDto.PhoneNumber,
                Address = createDto.Address,
                City = createDto.City,
                State = createDto.State,
                Country = createDto.Country,
                GSTNumber = createDto.GSTNumber,
                TaxID = createDto.TaxID,
                IsActive = createDto.IsActive,
                TenantId = normalizedTenantId,
                CreatedAt = DateTime.UtcNow
            };

            context.Vendors.Add(vendor);
            await context.SaveChangesAsync();

            return MapToDto(vendor);
        }

        public async Task<VendorDto?> UpdateVendorAsync(int id, UpdateVendorDto updateDto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var vendor = await context.Vendors
                .FirstOrDefaultAsync(v => v.Id == id && v.TenantId.ToLower() == normalizedTenantId);

            if (vendor == null) return null;

            // Update fields
            if (!string.IsNullOrWhiteSpace(updateDto.VendorName))
                vendor.VendorName = updateDto.VendorName;

            if (updateDto.ContactPerson != null)
                vendor.ContactPerson = updateDto.ContactPerson;

            if (updateDto.Email != null)
                vendor.Email = updateDto.Email;

            if (updateDto.PhoneNumber != null)
                vendor.PhoneNumber = updateDto.PhoneNumber;

            if (updateDto.Address != null)
                vendor.Address = updateDto.Address;

            if (updateDto.City != null)
                vendor.City = updateDto.City;

            if (updateDto.State != null)
                vendor.State = updateDto.State;

            if (updateDto.Country != null)
                vendor.Country = updateDto.Country;

            if (updateDto.GSTNumber != null)
                vendor.GSTNumber = updateDto.GSTNumber;

            if (updateDto.TaxID != null)
                vendor.TaxID = updateDto.TaxID;

            if (updateDto.IsActive.HasValue)
                vendor.IsActive = updateDto.IsActive.Value;

            vendor.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return MapToDto(vendor);
        }

        public async Task<bool> DeleteVendorAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var vendor = await context.Vendors
                .FirstOrDefaultAsync(v => v.Id == id && v.TenantId.ToLower() == normalizedTenantId);

            if (vendor == null) return false;

            context.Vendors.Remove(vendor);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<VendorDto>> GetActiveVendorsAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var vendors = await context.Vendors
                .Where(v => v.TenantId.ToLower() == normalizedTenantId && v.IsActive)
                .OrderBy(v => v.VendorName)
                .ToListAsync();

            return vendors.Select(MapToDto);
        }

        public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync(string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var vendors = await context.Vendors
                .Where(v => v.TenantId.ToLower() == normalizedTenantId)
                .OrderBy(v => v.VendorName)
                .ToListAsync();

            return vendors.Select(MapToDto);
        }

        private static VendorDto MapToDto(Vendor vendor)
        {
            return new VendorDto
            {
                Id = vendor.Id,
                VendorName = vendor.VendorName,
                ContactPerson = vendor.ContactPerson,
                Email = vendor.Email,
                PhoneNumber = vendor.PhoneNumber,
                Address = vendor.Address,
                City = vendor.City,
                State = vendor.State,
                Country = vendor.Country,
                GSTNumber = vendor.GSTNumber,
                TaxID = vendor.TaxID,
                IsActive = vendor.IsActive,
                CreatedAt = vendor.CreatedAt,
                UpdatedAt = vendor.UpdatedAt
            };
        }
    }
}

