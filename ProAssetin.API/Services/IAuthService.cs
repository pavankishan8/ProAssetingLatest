using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string? Token, string? Error, ApplicationUser? User)> LoginAsync(LoginDto loginDto);
        Task<(bool Success, string? Error)> RegisterAsync(RegisterDto registerDto);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetUsersByTenantAsync(string tenantId);
        Task<ApplicationUser?> SearchUserAsync(string searchTerm, string tenantId);
    }
}

