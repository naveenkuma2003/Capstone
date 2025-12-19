using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using DigitalBankLite.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalBankLite.API.Services
{
    public class TransferService : ITransferService
    {
        private readonly BankDbContext _context;

        public TransferService(BankDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, decimal? NewBalance)> TransferAsync(TransferDto dto, int userId)
        {
            if (dto.Amount <= 0)
            {
                return (false, "Amount must be greater than zero.", null);
            }

            // 1. Validate From Account
            var fromAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == dto.FromAccountId && a.CustomerId == userId);
            if (fromAccount == null)
            {
                return (false, "Invalid source account.", null);
            }

            if (fromAccount.Balance < dto.Amount)
            {
                return (false, "Insufficient balance.", null);
            }

            // 2. Validate To Account (Strictly own accounts for now per spec)
            var toAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == dto.ToAccountId && a.CustomerId == userId);
            if (toAccount == null)
            {
                 return (false, "Invalid destination account or it does not belong to you.", null);
            }

            if (fromAccount.Id == toAccount.Id)
            {
                 return (false, "Cannot transfer to the same account.", null);
            }

            // Execute Transfer (Transactional)
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Debit
                fromAccount.Balance -= dto.Amount;
                _context.Transactions.Add(new Transaction
                {
                    AccountId = fromAccount.Id,
                    TxnDateTime = DateTime.UtcNow,
                    Type = "Debit",
                    Amount = dto.Amount,
                    Description = $"Transfer to {toAccount.AccountNumber}: {dto.Remarks}",
                    BalanceAfterTxn = fromAccount.Balance
                });

                // Credit
                toAccount.Balance += dto.Amount;
                _context.Transactions.Add(new Transaction
                {
                    AccountId = toAccount.Id,
                    TxnDateTime = DateTime.UtcNow,
                    Type = "Credit",
                    Amount = dto.Amount,
                    Description = $"Transfer from {fromAccount.AccountNumber}: {dto.Remarks}",
                    BalanceAfterTxn = toAccount.Balance
                });

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return (true, "Transfer successful", fromAccount.Balance);
            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                // Log exception here
                throw; // Rethrow to be caught by middleware or controller
            }
        }
    }
}
