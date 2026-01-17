using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProAssetin.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ITenantResolver _tenantResolver;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ITenantResolver tenantResolver)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _tenantResolver = tenantResolver;
        }

        public async Task<(bool Success, string? Token, string? Error, ApplicationUser? User)> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null || !user.IsActive)
            {
                return (false, null, "Invalid email or password", null);
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                return (false, null, "Invalid email or password", null);
            }

            // Resolve tenant from email domain
            var tenantId = await _tenantResolver.ResolveTenantIdAsync(user.Email!);
            
            // Update tenant ID if not set
            if (string.IsNullOrEmpty(user.TenantId))
            {
                user.TenantId = tenantId;
                await _userManager.UpdateAsync(user);
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Generate JWT token
            var token = GenerateJwtToken(user, tenantId);

            return (true, token, null, user);
        }

        public async Task<(bool Success, string? Error)> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser is not null)
            {
                return (false, "User with this email already exists");
            }

            // Resolve tenant from email domain
            var tenantId = await _tenantResolver.ResolveTenantIdAsync(registerDto.Email);

            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            // Assign default role (User)
            await _userManager.AddToRoleAsync(user, "User");

            // Ensure tenant database exists
            await _tenantResolver.EnsureTenantDatabaseExistsAsync(tenantId);

            return (true, null);
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersByTenantAsync(string tenantId)
        {
            return _userManager.Users
                .Where(u => u.TenantId == tenantId && u.IsActive)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName);
        }

        public async Task<ApplicationUser?> SearchUserAsync(string searchTerm, string tenantId)
        {
            // Search by email or user ID
            var user = await _userManager.FindByEmailAsync(searchTerm);
            if (user != null && user.TenantId == tenantId && user.IsActive)
            {
                return user;
            }

            // Try to find by ID
            user = await _userManager.FindByIdAsync(searchTerm);
            if (user != null && user.TenantId == tenantId && user.IsActive)
            {
                return user;
            }

            // Search by name (FirstName or LastName contains searchTerm)
            var users = _userManager.Users
                .Where(u => u.TenantId == tenantId && u.IsActive && 
                           (u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm) || u.Email!.Contains(searchTerm)))
                .FirstOrDefault();

            return users;
        }

        private string GenerateJwtToken(ApplicationUser user, string tenantId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;
            var expirationMinutes = int.Parse(jwtSettings["ExpirationInMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim("TenantId", tenantId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims = claims.Append(new Claim(ClaimTypes.Role, role)).ToArray();
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

