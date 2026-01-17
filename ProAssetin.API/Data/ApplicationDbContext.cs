using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProAssetin.API.Models;

namespace ProAssetin.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<EmployeeConfiguration> EmployeeConfigurations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Rename Identity tables to ProAssetin naming convention
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Replace("AspNet", "ProAssetin"));
                }
            }

            // Configure ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("ProAssetinUsers");
                entity.HasIndex(e => e.TenantId);
                entity.HasIndex(e => e.Email);
            });

            // Configure IdentityRole
            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable("ProAssetinRoles");
            });

            // Configure IdentityUserRole
            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("ProAssetinUserRoles");
            });

            // Configure IdentityUserClaim
            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("ProAssetinUserClaims");
            });

            // Configure IdentityUserLogin
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("ProAssetinUserLogins");
            });

            // Configure IdentityRoleClaim
            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("ProAssetinRoleClaims");
            });

            // Configure IdentityUserToken
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("ProAssetinUserTokens");
            });

            // Configure Company
            builder.Entity<Company>(entity =>
            {
                entity.ToTable("ProAssetinCompanies");
                entity.HasKey(e => e.CompanyID);
                entity.Property(e => e.CompanyID).HasMaxLength(50);
                entity.Property(e => e.CompanyName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.PhoneNumber).HasMaxLength(50);
                entity.Property(e => e.Industry).HasMaxLength(100);
            });

            // Configure EmployeeConfiguration
            builder.Entity<EmployeeConfiguration>(entity =>
            {
                entity.ToTable("ProAssetinEmployeeConfigurations");
                entity.HasKey(e => e.ConfigurationID);
                entity.Property(e => e.ConfigurationID).ValueGeneratedOnAdd();
                entity.Property(e => e.PreDefinedAssetID).HasMaxLength(50);
                entity.Property(e => e.GSTNumber).HasMaxLength(50);
                entity.HasOne(e => e.Employee)
                    .WithMany()
                    .HasForeignKey(e => e.EmployeeID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure ApplicationUser -> Company relationship
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.CompanyID).HasMaxLength(50);
                entity.HasOne<Company>()
                    .WithMany()
                    .HasForeignKey(e => e.CompanyID)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}

