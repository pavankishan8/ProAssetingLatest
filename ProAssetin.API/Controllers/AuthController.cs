using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProAssetin.API.Models;
using ProAssetin.API.Models.DTOs;
using ProAssetin.API.Services;

namespace ProAssetin.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(loginDto);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Error });
            }

            return Ok(new
            {
                token = result.Token,
                user = new
                {
                    id = result.User!.Id,
                    email = result.User.Email,
                    firstName = result.User.FirstName,
                    lastName = result.User.LastName,
                    tenantId = result.User.TenantId
                }
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.Success)
            {
                return BadRequest(new { message = result.Error });
            }

            return Ok(new { message = "User registered successfully" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user is null)
            {
                return NotFound();
            }

            // At this point, user is guaranteed to be non-null after the check above
            ApplicationUser nonNullUser = user;
            return Ok(new
            {
                id = nonNullUser.Id,
                email = nonNullUser.Email,
                firstName = nonNullUser.FirstName,
                lastName = nonNullUser.LastName,
                tenantId = nonNullUser.TenantId
            });
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchUser([FromQuery] string searchTerm)
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Search term is required" });
            }

            var user = await _authService.SearchUserAsync(searchTerm, tenantId);
            if (user is null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new
            {
                id = user.Id,
                email = user.Email,
                firstName = user.FirstName,
                lastName = user.LastName,
                phoneNumber = user.PhoneNumber,
                location = user.Location,
                tenantId = user.TenantId
            });
        }

        [HttpGet("users")]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            var tenantId = HttpContext.Items["TenantId"]?.ToString();
            if (string.IsNullOrEmpty(tenantId))
            {
                return Unauthorized();
            }

            var users = await _authService.GetUsersByTenantAsync(tenantId);
            var userList = users.Select(u => new
            {
                id = u.Id,
                email = u.Email,
                firstName = u.FirstName,
                lastName = u.LastName,
                phoneNumber = u.PhoneNumber,
                location = u.Location,
                tenantId = u.TenantId
            });

            return Ok(userList);
        }
    }
}

