using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface ISecurityIncidentService
    {
        Task<(IEnumerable<SecurityIncidentDto> Items, int TotalCount)> GetIncidentsAsync(SecurityIncidentQueryDto query, string tenantId);
        Task<SecurityIncidentDto?> GetIncidentByIdAsync(int id, string tenantId);
        Task<SecurityIncidentDto> CreateIncidentAsync(CreateSecurityIncidentDto dto, string tenantId, string userId);
        Task<SecurityIncidentDto?> UpdateIncidentAsync(int id, UpdateSecurityIncidentDto dto, string tenantId);
        Task<bool> DeleteIncidentAsync(int id, string tenantId);
        Task<IEnumerable<string>> GetStatusesAsync();
        Task<IEnumerable<string>> GetSeveritiesAsync();
    }
}
