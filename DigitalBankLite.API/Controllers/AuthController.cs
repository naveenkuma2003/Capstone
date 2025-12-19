using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalBankLite.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Try NameIdentifier first, then sub
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                        ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdStr)) 
            {
                return Unauthorized(new { message = "User ID claim not found in token." });
            }

            if (!int.TryParse(userIdStr, out int userId))
            {
                return BadRequest(new { message = "Invalid User ID format in token." });
            }

            var profile = await _authService.GetProfileAsync(userId);
            
            if (profile == null) return NotFound(new { message = "Customer profile not found." });
            return Ok(profile);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result.Success)
            {
                return BadRequest(new { message = result.Message });
            }
            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Success)
            {
                // In a real app, distinguish 401 vs 404 vs 400
                return StatusCode(401, new { message = result.Message });
            }
            return Ok(result.Response);
        }

        [HttpGet("debug")]
        public async Task<IActionResult> DebugUsers()
        {
            var users = await _authService.DebugUsersAsync();
            return Ok(users);
        }

        [HttpGet("reset-admin-password")]
        public async Task<IActionResult> ResetAdminPassword()
        {
            await _authService.ResetAdminPasswordAsync();
            return Ok(new { message = "Password for myadmin@bank.com reset to 'Admin@123'" });
        }
    }
}
