using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IReportService _reportService;
        private readonly IVendorService _vendorService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ISoftwareService _softwareService;
        private readonly ITenantDbContextFactory _tenantDbFactory;

        public DashboardService(
            IReportService reportService,
            IVendorService vendorService,
            IPurchaseOrderService purchaseOrderService,
            ISoftwareService softwareService,
            ITenantDbContextFactory tenantDbFactory)
        {
            _reportService = reportService;
            _vendorService = vendorService;
            _purchaseOrderService = purchaseOrderService;
            _softwareService = softwareService;
            _tenantDbFactory = tenantDbFactory;
        }

        public async Task<object> GetDashboardDataAsync(string tenantId)
        {
            // Assets data
            var summary = await _reportService.GetAssetSummaryAsync(tenantId);
            var categoryStats = await _reportService.GetAssetStatsByCategoryAsync(tenantId);
            var statusStats = await _reportService.GetAssetStatsByStatusAsync(tenantId);

            // Convert arrays to dictionary/breakdown objects for frontend
            var categoryBreakdown = new Dictionary<string, int>();
            if (categoryStats is IEnumerable<object> categoryList)
            {
                foreach (var item in categoryList)
                {
                    if (item != null)
                    {
                        var categoryProp = item.GetType().GetProperty("Category");
                        var countProp = item.GetType().GetProperty("Count");
                        if (categoryProp != null && countProp != null)
                        {
                            var category = categoryProp.GetValue(item)?.ToString() ?? "Uncategorized";
                            var count = countProp.GetValue(item) is int c ? c : 0;
                            categoryBreakdown[category] = count;
                        }
                    }
                }
            }

            var statusBreakdown = new Dictionary<string, int>();
            if (statusStats is IEnumerable<object> statusList)
            {
                foreach (var item in statusList)
                {
                    if (item != null)
                    {
                        var statusProp = item.GetType().GetProperty("Status");
                        var countProp = item.GetType().GetProperty("Count");
                        if (statusProp != null && countProp != null)
                        {
                            var status = statusProp.GetValue(item)?.ToString() ?? "Unknown";
                            var count = countProp.GetValue(item) is int c ? c : 0;
                            statusBreakdown[status] = count;
                        }
                    }
                }
            }

            // Vendors data
            var vendorQuery = new VendorQueryDto { PageNumber = 1, PageSize = 1 };
            var (vendors, vendorCount) = await _vendorService.GetVendorsAsync(vendorQuery, tenantId);
            var allVendors = await _vendorService.GetAllVendorsAsync(tenantId);
            var activeVendorCount = allVendors.Count(v => v.IsActive);

            // Purchase Orders data
            var poQuery = new PurchaseOrderQueryDto { PageNumber = 1, PageSize = 1 };
            var (purchaseOrders, poCount) = await _purchaseOrderService.GetPurchaseOrdersAsync(poQuery, tenantId);
            var poStatusQuery = new PurchaseOrderQueryDto { PageNumber = 1, PageSize = 1000 }; // Get all for status breakdown
            var (allPOs, _) = await _purchaseOrderService.GetPurchaseOrdersAsync(poStatusQuery, tenantId);
            
            var poStatusBreakdown = allPOs
                .GroupBy(po => po.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            var approvedPOs = allPOs.Count(po => po.Status == "Approved");
            var draftPOs = allPOs.Count(po => po.Status == "Draft");
            var totalPOValue = allPOs.Sum(po => po.TotalAmount);

            // Software data
            var softwareQuery = new SoftwareQueryDto { PageNumber = 1, PageSize = 1 };
            var (software, softwareCount) = await _softwareService.GetSoftwareAsync(softwareQuery, tenantId);
            var allSoftwareQuery = new SoftwareQueryDto { PageNumber = 1, PageSize = 1000 }; // Get all for breakdown
            var (allSoftware, _) = await _softwareService.GetSoftwareAsync(allSoftwareQuery, tenantId);
            
            var softwareStatusBreakdown = allSoftware
                .GroupBy(s => s.Status)
                .ToDictionary(g => g.Key, g => g.Count());

            var activeSoftware = allSoftware.Count(s => s.Status == "Active");
            var expiredSoftware = allSoftware.Count(s => s.Status == "Expired");
            var totalSoftwareValue = allSoftware.Where(s => s.PurchasePrice.HasValue).Sum(s => s.PurchasePrice!.Value);

            var t = tenantId?.ToLowerInvariant() ?? string.Empty;
            int invoiceTotal = 0, invoicePaid = 0, invoicePending = 0, invoiceOverdue = 0;
            decimal invoiceAmountOutstanding = 0;
            int budgetTotal = 0, budgetActive = 0;
            decimal budgetAllocatedSum = 0, budgetSpentSum = 0;
            int ewasteTotal = 0;
            int securityTotal = 0, securityOpen = 0;
            int projectTotal = 0;
            int contractTotal = 0;
            int ticketTotal = 0;
            int ticketOpen = 0;

            await using (var ctx = _tenantDbFactory.CreateDbContext(tenantId))
            {
                var invoices = ctx.Invoices.Where(i => i.TenantId.ToLower() == t);
                invoiceTotal = await invoices.CountAsync();
                invoicePaid = await invoices.CountAsync(i => i.Status == "Paid");
                invoicePending = await invoices.CountAsync(i => i.Status == "Pending");
                invoiceOverdue = await invoices.CountAsync(i => i.Status == "Overdue");
                invoiceAmountOutstanding = await invoices
                    .Where(i => i.Status != "Paid" && i.Status != "Cancelled")
                    .SumAsync(i => (decimal?)i.Amount) ?? 0;

                var budgets = ctx.Budgets.Where(b => b.TenantId.ToLower() == t);
                budgetTotal = await budgets.CountAsync();
                budgetActive = await budgets.CountAsync(b => b.Status == "Active");
                budgetAllocatedSum = await budgets.SumAsync(b => (decimal?)b.AllocatedAmount) ?? 0;
                budgetSpentSum = await budgets.SumAsync(b => (decimal?)b.SpentAmount) ?? 0;

                ewasteTotal = await ctx.EWasteDisposals.CountAsync(e => e.TenantId.ToLower() == t);

                var incidents = ctx.SecurityIncidents.Where(s => s.TenantId.ToLower() == t);
                securityTotal = await incidents.CountAsync();
                securityOpen = await incidents.CountAsync(s =>
                    s.Status == "Open" || s.Status == "Investigating" || s.Status == "In Progress");

                projectTotal = await ctx.Projects.CountAsync(p => p.TenantId.ToLower() == t);
                contractTotal = await ctx.Contracts.CountAsync(c => c.TenantId.ToLower() == t);

                var tickets = ctx.Tickets.Where(x => x.TenantId.ToLower() == t);
                ticketTotal = await tickets.CountAsync();
                ticketOpen = await tickets.CountAsync(x =>
                    x.TaskState == "Open" || x.TaskState == "In Progress");
            }

            return new
            {
                summary = summary,
                categoryStats = categoryStats,
                statusStats = statusStats,
                categoryBreakdown = categoryBreakdown,
                statusBreakdown = statusBreakdown,
                // Vendors summary
                vendors = new
                {
                    total = vendorCount,
                    active = activeVendorCount,
                    inactive = vendorCount - activeVendorCount
                },
                // Purchase Orders summary
                purchaseOrders = new
                {
                    total = poCount,
                    approved = approvedPOs,
                    draft = draftPOs,
                    totalValue = totalPOValue,
                    statusBreakdown = poStatusBreakdown
                },
                // Software summary
                software = new
                {
                    total = softwareCount,
                    active = activeSoftware,
                    expired = expiredSoftware,
                    totalValue = totalSoftwareValue,
                    statusBreakdown = softwareStatusBreakdown
                },
                invoices = new
                {
                    total = invoiceTotal,
                    paid = invoicePaid,
                    pending = invoicePending,
                    overdue = invoiceOverdue,
                    outstandingAmount = invoiceAmountOutstanding
                },
                budgets = new
                {
                    total = budgetTotal,
                    active = budgetActive,
                    allocated = budgetAllocatedSum,
                    spent = budgetSpentSum
                },
                ewaste = new { total = ewasteTotal },
                security = new { total = securityTotal, open = securityOpen },
                projects = new { total = projectTotal },
                contracts = new { total = contractTotal },
                tickets = new { total = ticketTotal, open = ticketOpen }
            };
        }
    }
}

