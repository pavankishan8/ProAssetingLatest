using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface ITicketService
    {
        Task<(IEnumerable<TicketDto> Items, int TotalCount)> GetTicketsAsync(TicketQueryDto query, string tenantId);
        Task<TicketDto?> GetTicketByIdAsync(int id, string tenantId);
        Task<TicketDto> CreateTicketAsync(CreateTicketDto dto, string tenantId, string userId);
        Task<TicketDto?> UpdateTicketAsync(int id, UpdateTicketDto dto, string tenantId);
        Task<bool> DeleteTicketAsync(int id, string tenantId);
        Task<IEnumerable<string>> GetTaskStatesAsync();
        Task<IEnumerable<string>> GetPrioritiesAsync();
    }
}
