using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProAssetin.API.Data;

namespace ProAssetin.API.Services
{
    public class TenantResolver : ITenantResolver
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _masterContext;

        public TenantResolver(IConfiguration configuration, ApplicationDbContext masterContext)
        {
            _configuration = configuration;
            _masterContext = masterContext;
        }

        public Task<string> ResolveTenantIdAsync(string email)
        {
            // Extract domain from email (e.g., pavan@infosys.com -> infosys)
            var emailParts = email.Split('@');
            if (emailParts.Length != 2)
            {
                throw new ArgumentException("Invalid email format", nameof(email));
            }

            var domain = emailParts[1].Split('.')[0].ToLower(); // Get first part of domain
            return Task.FromResult(domain);
        }

        public string GetDatabaseName(string tenantId)
        {
            var prefix = _configuration["TenantSettings:DefaultDatabasePrefix"] ?? "ProAsset_";
            return $"{prefix}{tenantId}";
        }

        public async Task<bool> EnsureTenantDatabaseExistsAsync(string tenantId)
        {
            try
            {
                var databaseName = GetDatabaseName(tenantId);
                var tenantConnectionString = GetTenantConnectionString(databaseName);

                var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
                optionsBuilder.UseSqlServer(tenantConnectionString);

                using var context = new TenantDbContext(optionsBuilder.Options);
                
                // Check if database exists by attempting to connect
                var canConnect = await context.Database.CanConnectAsync();
                
                if (!canConnect)
                {
                    // Create database and run migrations
                    await context.Database.EnsureCreatedAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetTenantConnectionString(string databaseName)
        {
            // Map tenant ID to connection string name
            var tenantIdLower = databaseName.Replace("ProAsset_", "").ToLower();
            
            // Check for specific customer connection strings (Infosys, Wipro)
            var connectionStringName = tenantIdLower switch
            {
                "infosys" => "Infosys",
                "wipro" => "Wipro",
                _ => null
            };

            if (!string.IsNullOrEmpty(connectionStringName))
            {
                var connectionString = _configuration.GetConnectionString(connectionStringName);
                if (!string.IsNullOrEmpty(connectionString))
                {
                    return connectionString;
                }
            }

            // Fallback: build from master connection if customer connection not found
            var masterConnection = _configuration.GetConnectionString("MasterConnection");
            return masterConnection!.Replace("Database=ProAssetinDev", $"Database={databaseName}");
        }
    }
}

