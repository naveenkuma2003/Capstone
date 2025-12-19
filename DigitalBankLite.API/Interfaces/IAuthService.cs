using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Models;

namespace DigitalBankLite.API.Interfaces
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, Customer? Customer)> RegisterAsync(RegisterDto dto);
        Task<(bool Success, string Message, LoginResponseDto? Response)> LoginAsync(LoginDto dto);
        Task SeedAdminAsync();
        Task ResetAdminPasswordAsync();
        Task<IEnumerable<object>> DebugUsersAsync();
        Task<object?> GetProfileAsync(int userId);
    }
}
