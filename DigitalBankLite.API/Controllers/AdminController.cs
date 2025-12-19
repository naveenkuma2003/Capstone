using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBankLite.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("customers/pending")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            var result = await _adminService.GetPendingApprovalsAsync();
            return Ok(result);
        }

        [HttpPut("customers/{id}/approve")]
        public async Task<IActionResult> ApproveCustomer(int id)
        {
            var result = await _adminService.ApproveCustomerAsync(id);
            if (!result.Success) return NotFound(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPut("customers/{id}/reject")]
        public async Task<IActionResult> RejectCustomer(int id)
        {
            var result = await _adminService.RejectCustomerAsync(id);
            if (!result.Success) return NotFound(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> GetAllAccounts()
        {
            var result = await _adminService.GetAllAccountsAsync();
            return Ok(result);
        }

        [HttpDelete("accounts/{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var result = await _adminService.DeleteAccountAsync(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpGet("transactions/highvalue")]
        public async Task<IActionResult> GetHighValueTransactions([FromQuery] decimal threshold = 100000)
        {
            var result = await _adminService.GetHighValueTransactionsAsync(threshold);
            return Ok(result);
        }

        [HttpGet("servicerequests")]
        public async Task<IActionResult> GetAllServiceRequests()
        {
            var result = await _adminService.GetAllServiceRequestsAsync();
            return Ok(result);
        }

        [HttpPut("servicerequests/{id}")]
        public async Task<IActionResult> UpdateServiceRequestStatus(int id, [FromBody] UpdateServiceRequestStatusDto dto)
        {
            var result = await _adminService.UpdateServiceRequestStatusAsync(id, dto.Status);
            if (!result.Success) return NotFound(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("servicerequests/{id}")]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var result = await _adminService.DeleteServiceRequestAsync(id);
             if (!result.Success) 
             {
                 if (result.Message == "Request not found.") return NotFound(new { message = result.Message });
                 return BadRequest(new { message = result.Message });
             }
            return Ok(new { message = result.Message });
        }
    }
}
