using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankLite.API.Services
{
    public class AdminService : IAdminService
    {
        private readonly BankDbContext _context;

        public AdminService(BankDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetPendingApprovalsAsync()
        {
            return await _context.Customers
                .Where(c => c.KycStatus == "Pending" && c.Role != "Admin")
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> ApproveCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return (false, "Customer not found.");

            customer.KycStatus = "Approved";

            // Activate all their accounts
            var accounts = await _context.Accounts.Where(a => a.CustomerId == id && a.Status == "Inactive").ToListAsync();
            foreach (var acc in accounts)
            {
                acc.Status = "Active";
                acc.Balance = 0; 
            }

            await _context.SaveChangesAsync();
            return (true, "Customer approved and accounts activated.");
        }

        public async Task<(bool Success, string Message)> RejectCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null) return (false, "Customer not found.");

            customer.KycStatus = "Rejected";
            await _context.SaveChangesAsync();
            return (true, "Customer KYC rejected.");
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts
                .Include(a => a.Customer)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> DeleteAccountAsync(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return (false, "Account not found.");

            if (account.Balance > 0)
            {
                return (false, "Cannot delete an account with a non-zero balance.");
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return (true, "Account deleted successfully.");
        }

        public async Task<IEnumerable<Transaction>> GetHighValueTransactionsAsync(decimal threshold)
        {
            return await _context.Transactions
                .Include(t => t.Account)
                .ThenInclude(a => a!.Customer)
                .Where(t => t.Amount > threshold)
                .OrderByDescending(t => t.TxnDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllServiceRequestsAsync()
        {
            return await _context.ServiceRequests
                .Include(r => r.Customer)
                .OrderByDescending(r => r.CreatedDate)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> UpdateServiceRequestStatusAsync(int id, string status)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return (false, "Request not found.");

            request.Status = status;
            request.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Status updated.");
        }

        public async Task<(bool Success, string Message)> DeleteServiceRequestAsync(int id)
        {
            var request = await _context.ServiceRequests.FindAsync(id);
            if (request == null) return (false, "Request not found.");

            if (request.Status != "Closed")
            {
                return (false, "Only closed requests can be deleted.");
            }

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return (true, "Service request deleted.");
        }
    }
}
