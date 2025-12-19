using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Models;

namespace DigitalBankLite.API.Interfaces
{
    public interface IAdminService
    {
        Task<IEnumerable<Customer>> GetPendingApprovalsAsync();
        Task<(bool Success, string Message)> ApproveCustomerAsync(int id);
        Task<(bool Success, string Message)> RejectCustomerAsync(int id);
        Task<IEnumerable<Account>> GetAllAccountsAsync();
        Task<(bool Success, string Message)> DeleteAccountAsync(int id);
        Task<IEnumerable<Transaction>> GetHighValueTransactionsAsync(decimal threshold);
        Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync();
        Task<(bool Success, string Message)> UpdateServiceRequestStatusAsync(int id, string status);
        Task<(bool Success, string Message)> DeleteServiceRequestAsync(int id);
    }
}
