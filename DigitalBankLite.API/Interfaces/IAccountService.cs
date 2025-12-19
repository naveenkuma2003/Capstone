using DigitalBankLite.API.Models;

namespace DigitalBankLite.API.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<Account>> GetAccountsAsync(int userId);
        Task<IEnumerable<Transaction>> GetTransactionsAsync(int accountId, int userId);
        Task<IEnumerable<object>> GetAllTransactionsAsync(int userId);
        Task<IEnumerable<object>> GetMiniStatementAsync(int userId);
        Task<byte[]> GetDownloadStatementAsync(int userId);
        Task<(bool Success, string Message, Transaction? Transaction)> DepositAsync(int accountId, decimal amount, int userId);
    }
}
