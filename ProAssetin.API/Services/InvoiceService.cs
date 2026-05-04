using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Data;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ITenantDbContextFactory _dbContextFactory;
        private readonly ApplicationDbContext _masterContext;

        public InvoiceService(ITenantDbContextFactory dbContextFactory, ApplicationDbContext masterContext)
        {
            _dbContextFactory = dbContextFactory;
            _masterContext = masterContext;
        }

        public async Task<(IEnumerable<InvoiceDto> Invoices, int TotalCount)> GetInvoicesAsync(InvoiceQueryDto query, string tenantId)
        {
            try
            {
                using var context = _dbContextFactory.CreateDbContext(tenantId);

                var normalizedTenantId = tenantId?.ToLowerInvariant();

                var invoicesQuery = context.Invoices
                    .Where(inv => inv.TenantId != null && inv.TenantId.ToLower() == normalizedTenantId)
                    .AsQueryable();

                // Apply filters
                if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                {
                    invoicesQuery = invoicesQuery.Where(inv =>
                        inv.InvoiceNumber.Contains(query.SearchTerm) ||
                        inv.VendorName!.Contains(query.SearchTerm) ||
                        inv.Description!.Contains(query.SearchTerm) ||
                        inv.PurchaseOrderNumber!.Contains(query.SearchTerm));
                }

                if (!string.IsNullOrWhiteSpace(query.Status))
                {
                    invoicesQuery = invoicesQuery.Where(inv => inv.Status == query.Status);
                }

                if (!string.IsNullOrWhiteSpace(query.VendorName))
                {
                    invoicesQuery = invoicesQuery.Where(inv => inv.VendorName == query.VendorName);
                }

                if (query.InvoiceDateFrom.HasValue)
                {
                    invoicesQuery = invoicesQuery.Where(inv => inv.InvoiceDate >= query.InvoiceDateFrom.Value);
                }

                if (query.InvoiceDateTo.HasValue)
                {
                    invoicesQuery = invoicesQuery.Where(inv => inv.InvoiceDate <= query.InvoiceDateTo.Value);
                }

                // Get total count before pagination
                int totalCount;
                try
                {
                    totalCount = await invoicesQuery.CountAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error counting invoices: {ex.Message}", ex);
                }

                // Apply sorting
                invoicesQuery = query.SortBy?.ToLower() switch
                {
                    "invoicenumber" => query.SortDescending ? invoicesQuery.OrderByDescending(inv => inv.InvoiceNumber) : invoicesQuery.OrderBy(inv => inv.InvoiceNumber),
                    "invoicedate" => query.SortDescending ? invoicesQuery.OrderByDescending(inv => inv.InvoiceDate) : invoicesQuery.OrderBy(inv => inv.InvoiceDate),
                    "amount" => query.SortDescending ? invoicesQuery.OrderByDescending(inv => inv.Amount) : invoicesQuery.OrderBy(inv => inv.Amount),
                    "status" => query.SortDescending ? invoicesQuery.OrderByDescending(inv => inv.Status) : invoicesQuery.OrderBy(inv => inv.Status),
                    "vendor" => query.SortDescending ? invoicesQuery.OrderByDescending(inv => inv.VendorName) : invoicesQuery.OrderBy(inv => inv.VendorName),
                    _ => query.SortDescending ? invoicesQuery.OrderByDescending(inv => inv.InvoiceDate) : invoicesQuery.OrderByDescending(inv => inv.InvoiceDate)
                };

                // Apply pagination
                List<Invoice> invoices;
                try
                {
                    invoices = await invoicesQuery
                        .Skip((query.PageNumber - 1) * query.PageSize)
                        .Take(query.PageSize)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving paginated invoices: {ex.Message}", ex);
                }

                // Get unique user IDs
                var userIds = invoices
                    .Where(inv => !string.IsNullOrEmpty(inv.CreatedByUserId))
                    .Select(inv => inv.CreatedByUserId!)
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

                // Map to DTOs
                IEnumerable<InvoiceDto> invoiceDtos;
                try
                {
                    invoiceDtos = invoices.Select(inv => MapToDto(inv, userNamesDict));
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error mapping invoices to DTOs: {ex.Message}", ex);
                }

                return (invoiceDtos, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in GetInvoicesAsync for tenant {tenantId}: {ex.Message}", ex);
            }
        }

        public async Task<InvoiceDto?> GetInvoiceByIdAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var invoice = await context.Invoices
                .FirstOrDefaultAsync(inv => inv.Id == id && inv.TenantId.ToLower() == normalizedTenantId);

            if (invoice == null) return null;

            // Fetch user name from master database if created
            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(invoice.CreatedByUserId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == invoice.CreatedByUserId)
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

            return MapToDto(invoice, userNamesDict);
        }

        public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createDto, string tenantId, string userId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLowerInvariant();

            var invoice = new Invoice
            {
                InvoiceNumber = createDto.InvoiceNumber,
                VendorName = createDto.VendorName,
                Amount = createDto.Amount,
                InvoiceDate = createDto.InvoiceDate,
                DueDate = createDto.DueDate,
                Status = createDto.Status,
                Description = createDto.Description,
                PurchaseOrderNumber = createDto.PurchaseOrderNumber,
                TenantId = normalizedTenantId,
                CreatedByUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            // Fetch user name
            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == userId)
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

            return MapToDto(invoice, userNamesDict);
        }

        public async Task<InvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateDto, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var invoice = await context.Invoices
                .FirstOrDefaultAsync(inv => inv.Id == id && inv.TenantId.ToLower() == normalizedTenantId);

            if (invoice == null) return null;

            // Update fields
            if (!string.IsNullOrWhiteSpace(updateDto.InvoiceNumber))
                invoice.InvoiceNumber = updateDto.InvoiceNumber;

            if (updateDto.VendorName != null)
                invoice.VendorName = updateDto.VendorName;

            if (updateDto.Amount.HasValue)
                invoice.Amount = updateDto.Amount.Value;

            if (updateDto.InvoiceDate.HasValue)
                invoice.InvoiceDate = updateDto.InvoiceDate.Value;

            if (updateDto.DueDate != null)
                invoice.DueDate = updateDto.DueDate;

            if (!string.IsNullOrWhiteSpace(updateDto.Status))
                invoice.Status = updateDto.Status;

            if (updateDto.Description != null)
                invoice.Description = updateDto.Description;

            if (updateDto.PurchaseOrderNumber != null)
                invoice.PurchaseOrderNumber = updateDto.PurchaseOrderNumber;

            invoice.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            // Fetch user name
            Dictionary<string, string>? userNamesDict = null;
            if (!string.IsNullOrEmpty(invoice.CreatedByUserId))
            {
                var user = await _masterContext.Users
                    .Where(u => u.Id == invoice.CreatedByUserId)
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

            return MapToDto(invoice, userNamesDict);
        }

        public async Task<bool> DeleteInvoiceAsync(int id, string tenantId)
        {
            using var context = _dbContextFactory.CreateDbContext(tenantId);

            var normalizedTenantId = tenantId?.ToLower();

            var invoice = await context.Invoices
                .FirstOrDefaultAsync(inv => inv.Id == id && inv.TenantId.ToLower() == normalizedTenantId);

            if (invoice == null) return false;

            context.Invoices.Remove(invoice);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<string>> GetInvoiceStatusesAsync()
        {
            return await Task.FromResult(new[]
            {
                "Pending",
                "Paid",
                "Overdue",
                "Cancelled"
            });
        }

        private static InvoiceDto MapToDto(Invoice invoice, Dictionary<string, string>? userNamesDict = null)
        {
            string? createdByUserName = null;
            if (!string.IsNullOrEmpty(invoice.CreatedByUserId) && userNamesDict != null)
            {
                userNamesDict.TryGetValue(invoice.CreatedByUserId, out createdByUserName);
            }

            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                VendorName = invoice.VendorName,
                Amount = invoice.Amount,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                Status = invoice.Status,
                Description = invoice.Description,
                PurchaseOrderNumber = invoice.PurchaseOrderNumber,
                CreatedByUserId = invoice.CreatedByUserId,
                CreatedByUserName = createdByUserName,
                CreatedAt = invoice.CreatedAt,
                UpdatedAt = invoice.UpdatedAt
            };
        }
    }
}

