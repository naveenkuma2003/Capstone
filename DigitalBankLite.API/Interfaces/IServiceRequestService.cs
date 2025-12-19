using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Models;

namespace DigitalBankLite.API.Interfaces
{
    public interface IServiceRequestService
    {
        Task<(bool Success, string Message)> CreateServiceRequestAsync(CreateServiceRequestDto dto, int userId);
        Task<IEnumerable<ServiceRequest>> GetMyRequestsAsync(int userId);
    }
}
