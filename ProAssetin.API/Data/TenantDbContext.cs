using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Models;

namespace ProAssetin.API.Data
{
    public class TenantDbContext : DbContext
    {
        public TenantDbContext(DbContextOptions<TenantDbContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Software> Software { get; set; }
        public DbSet<CompanySettings> CompanySettings { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<EWasteDisposal> EWasteDisposals { get; set; }
        public DbSet<SecurityIncident> SecurityIncidents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure table names with ProAssetin prefix
            builder.Entity<Asset>().ToTable("ProAssetinAssets");
            builder.Entity<InventoryLog>().ToTable("ProAssetinInventoryLogs");
            builder.Entity<Invoice>().ToTable("ProAssetinInvoices");
            builder.Entity<PurchaseOrder>().ToTable("ProAssetinPurchaseOrders");
            builder.Entity<Ticket>().ToTable("ProAssetinTickets");
            builder.Entity<Vendor>().ToTable("ProAssetinVendors");
            builder.Entity<Software>().ToTable("ProAssetinSoftware");
            builder.Entity<CompanySettings>().ToTable("ProAssetinCompanySettings");
            builder.Entity<Budget>().ToTable("ProAssetinBudgets");
            builder.Entity<EWasteDisposal>().ToTable("ProAssetinEWasteDisposals");
            builder.Entity<SecurityIncident>().ToTable("ProAssetinSecurityIncidents");

            // Configure Asset entity
            builder.Entity<Asset>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.AssetId, e.TenantId }).IsUnique();
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Category);
                entity.Property(e => e.AssetId).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            });

            // Configure InventoryLog entity
            builder.Entity<InventoryLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.AssetId);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.CreatedAt);
                entity.HasOne(e => e.Asset)
                    .WithMany()
                    .HasForeignKey(e => e.AssetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Invoice entity
            builder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.InvoiceNumber).IsUnique();
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.InvoiceDate);
                entity.Property(e => e.InvoiceNumber).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            });

            // Configure PurchaseOrder entity
            builder.Entity<PurchaseOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.PONumber).IsUnique();
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Status);
                entity.Property(e => e.PONumber).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            });

            // Configure Ticket entity
            builder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.TaskID);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.TaskState);
                entity.Property(e => e.TaskTitle).HasMaxLength(500).IsRequired();
                entity.Property(e => e.TaskState).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Priority).HasMaxLength(50);
            });

            // Configure Vendor entity
            builder.Entity<Vendor>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.VendorName);
                entity.Property(e => e.VendorName).HasMaxLength(200).IsRequired();
            });

            // Configure Software entity
            builder.Entity<Software>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.SoftwareName);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Category);
                entity.HasIndex(e => e.VendorId);
                entity.Property(e => e.SoftwareName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
                entity.HasOne(e => e.Vendor)
                    .WithMany()
                    .HasForeignKey(e => e.VendorId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure CompanySettings entity
            builder.Entity<CompanySettings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TenantId).IsUnique(); // One settings per tenant
                entity.Property(e => e.TenantId).HasMaxLength(100).IsRequired();
                entity.Property(e => e.CompanyName).HasMaxLength(200).IsRequired();
            });

            builder.Entity<Budget>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.FiscalYear);
                entity.HasIndex(e => e.Status);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            });

            builder.Entity<EWasteDisposal>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => new { e.DisposalReference, e.TenantId }).IsUnique();
                entity.HasIndex(e => e.DisposalDate);
                entity.Property(e => e.DisposalReference).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ItemDescription).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
                entity.HasOne(e => e.Asset)
                    .WithMany()
                    .HasForeignKey(e => e.AssetId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<SecurityIncident>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.Severity);
                entity.HasIndex(e => e.ReportedDate);
                entity.HasIndex(e => new { e.IncidentReference, e.TenantId }).IsUnique();
                entity.Property(e => e.IncidentReference).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Severity).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
            });
        }
    }
}
