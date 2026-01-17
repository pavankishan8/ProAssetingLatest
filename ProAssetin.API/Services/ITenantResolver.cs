namespace ProAssetin.API.Services
{
    public interface ITenantResolver
    {
        Task<string> ResolveTenantIdAsync(string email);
        string GetDatabaseName(string tenantId);
        string GetTenantConnectionString(string databaseName);
        Task<bool> EnsureTenantDatabaseExistsAsync(string tenantId);
    }
}

