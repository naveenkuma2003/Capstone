using DigitalBankLite.API.DTOs;

namespace DigitalBankLite.API.Interfaces
{
    public interface ITransferService
    {
        Task<(bool Success, string Message, decimal? NewBalance)> TransferAsync(TransferDto dto, int userId);
    }
}
