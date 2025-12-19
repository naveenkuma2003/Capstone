using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DigitalBankLite.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly BankDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(BankDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, Customer? Customer)> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == dto.Email))
            {
                return (false, "Email already registered.", null);
            }

            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedDate = DateTime.UtcNow,
                Role = "Customer",
                KycStatus = "Pending"
            };

            if (dto.Email.Contains("admin"))
            {
                customer.Role = "Admin";
                customer.KycStatus = "Approved";
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // Create default accounts
            var savingsAccount = new Account
            {
                CustomerId = customer.Id,
                AccountNumber = GenerateAccountNumber(),
                AccountType = "Savings",
                Balance = 0,
                Status = "Inactive"
            };
            _context.Accounts.Add(savingsAccount);

            var currentAccount = new Account
            {
                CustomerId = customer.Id,
                AccountNumber = GenerateAccountNumber(),
                AccountType = "Current",
                Balance = 0,
                Status = "Inactive"
            };
            _context.Accounts.Add(currentAccount);
            await _context.SaveChangesAsync();

            return (true, "Registration successful.", customer);
        }

        public async Task<(bool Success, string Message, LoginResponseDto? Response)> LoginAsync(LoginDto dto)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == dto.Email);
            if (customer == null || !BCrypt.Net.BCrypt.Verify(dto.Password, customer.PasswordHash))
            {
                return (false, "Invalid credentials.", null);
            }

            if (customer.Role != "Admin" && customer.KycStatus != "Approved")
            {
                return (false, "Account is not approved yet. KYC Pending.", null);
            }

            var token = GenerateJwtToken(customer);
            return (true, "Login successful.", new LoginResponseDto
            {
                Token = token,
                Name = customer.Name,
                Email = customer.Email,
                Role = customer.Role
            });
        }

        private string GenerateJwtToken(Customer user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task ResetAdminPasswordAsync()
        {
            var admin = await _context.Customers.FirstOrDefaultAsync(c => c.Email == "myadmin@bank.com");
            if (admin != null)
            {
                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<object>> DebugUsersAsync()
        {
             return await _context.Customers.Select(c => new { c.Email, c.Role, c.KycStatus, c.PasswordHash }).ToListAsync();
        }

        public async Task SeedAdminAsync()
        {
            var adminEmail = "admin@digitalbank.com";
            if (!await _context.Customers.AnyAsync(c => c.Email == adminEmail))
            {
                var admin = new Customer
                {
                    Name = "System Admin",
                    Email = adminEmail,
                    Phone = "0000000000",
                    Address = "System",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    Role = "Admin",
                    KycStatus = "Approved",
                    CreatedDate = DateTime.UtcNow
                };
                _context.Customers.Add(admin);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<object?> GetProfileAsync(int userId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == userId);
            if (customer == null) return null;

            return new
            {
                customer.Name,
                customer.Email,
                customer.Phone,
                customer.Address,
                customer.Role,
                customer.KycStatus,
                customer.CreatedDate
            };
        }

        private string GenerateAccountNumber()
        {
            var random = new Random();
            return "DB" + random.Next(10000000, 99999999).ToString();
        }
    }
}
