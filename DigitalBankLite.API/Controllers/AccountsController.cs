using DigitalBankLite.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalBankLite.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var accounts = await _accountService.GetAccountsAsync(userId);
            return Ok(accounts);
        }

        [HttpGet("{accountId}/transactions")]
        public async Task<IActionResult> GetTransactions(int accountId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var transactions = await _accountService.GetTransactionsAsync(accountId, userId);
            return Ok(transactions);
        }
        
        [HttpGet("transactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var transactions = await _accountService.GetAllTransactionsAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("mini-statement")]
        public async Task<IActionResult> GetMiniStatement()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var transactions = await _accountService.GetMiniStatementAsync(userId);
            return Ok(transactions);
        }

        [HttpGet("download-statement")]
        public async Task<IActionResult> DownloadStatement()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var csvData = await _accountService.GetDownloadStatementAsync(userId);
            var fileName = $"Statement_{DateTime.Now:yyyyMMdd}.csv";
            return File(csvData, "text/csv", fileName);
        }

        [HttpPost("{accountId}/deposit")]
        public async Task<IActionResult> Deposit(int accountId, [FromBody] decimal amount)
        {
             var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
             var result = await _accountService.DepositAsync(accountId, amount, userId);
             
             if (!result.Success)
             {
                 return BadRequest(new { message = result.Message });
             }
             return Ok(new { message = result.Message, newBalance = result.Transaction?.BalanceAfterTxn });
        }
    }
}
