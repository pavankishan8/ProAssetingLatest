using ProAssetin.API.Data;

namespace ProAssetin.API.Services
{
    public interface ITenantDbContextFactory
    {
        TenantDbContext CreateDbContext(string tenantId);
    }
}

