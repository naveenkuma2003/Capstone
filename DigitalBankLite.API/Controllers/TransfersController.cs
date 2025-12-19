using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalBankLite.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransfersController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransferDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            
            try 
            {
                var result = await _transferService.TransferAsync(dto, userId);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }
                return Ok(new { message = result.Message, newBalance = result.NewBalance });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred during transfer." });
            }
        }
    }
}
