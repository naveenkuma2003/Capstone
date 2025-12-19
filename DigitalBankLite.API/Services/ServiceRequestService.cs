using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankLite.API.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly BankDbContext _context;

        public ServiceRequestService(BankDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message)> CreateServiceRequestAsync(CreateServiceRequestDto dto, int userId)
        {
            var request = new ServiceRequest
            {
                CustomerId = userId,
                Title = dto.Title,
                Description = dto.Description,
                Status = "Open",
                CreatedDate = DateTime.UtcNow
            };

            _context.ServiceRequests.Add(request);
            await _context.SaveChangesAsync();

            return (true, "Service request created successfully.");
        }

        public async Task<IEnumerable<ServiceRequest>> GetMyRequestsAsync(int userId)
        {
            return await _context.ServiceRequests
                .Where(r => r.CustomerId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }
    }
}
