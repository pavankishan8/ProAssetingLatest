using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface IInvoiceService
    {
        Task<(IEnumerable<InvoiceDto> Invoices, int TotalCount)> GetInvoicesAsync(InvoiceQueryDto query, string tenantId);
        Task<InvoiceDto?> GetInvoiceByIdAsync(int id, string tenantId);
        Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createDto, string tenantId, string userId);
        Task<InvoiceDto?> UpdateInvoiceAsync(int id, UpdateInvoiceDto updateDto, string tenantId);
        Task<bool> DeleteInvoiceAsync(int id, string tenantId);
        Task<IEnumerable<string>> GetInvoiceStatusesAsync();
    }
}

