namespace ProAssetin.API.Services
{
    public interface IDashboardService
    {
        Task<object> GetDashboardDataAsync(string tenantId);
    }
}

