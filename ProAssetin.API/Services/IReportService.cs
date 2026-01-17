namespace ProAssetin.API.Services
{
    public interface IReportService
    {
        Task<object> GetAssetSummaryAsync(string tenantId);
        Task<object> GetAssetStatsByCategoryAsync(string tenantId);
        Task<object> GetAssetStatsByStatusAsync(string tenantId);
    }
}

