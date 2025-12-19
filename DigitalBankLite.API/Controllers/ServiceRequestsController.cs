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
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _requestService;

        public ServiceRequestsController(IServiceRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceRequest(CreateServiceRequestDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _requestService.CreateServiceRequestAsync(dto, userId);
            
            return Ok(new { message = result.Message });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var result = await _requestService.GetMyRequestsAsync(userId);
            return Ok(result);
        }
    }
}
