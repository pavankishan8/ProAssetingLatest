using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProAssetin.API.Data;

namespace ProAssetin.API.Services
{
    public class TenantDbContextFactory : ITenantDbContextFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ITenantResolver _tenantResolver;

        public TenantDbContextFactory(IConfiguration configuration, ITenantResolver tenantResolver)
        {
            _configuration = configuration;
            _tenantResolver = tenantResolver;
        }

        public TenantDbContext CreateDbContext(string tenantId)
        {
            var databaseName = _tenantResolver.GetDatabaseName(tenantId);
            var tenantConnectionString = _tenantResolver.GetTenantConnectionString(databaseName);

            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(tenantConnectionString);

            return new TenantDbContext(optionsBuilder.Options);
        }
    }
}

