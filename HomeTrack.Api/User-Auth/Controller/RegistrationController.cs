using Microsoft.AspNetCore.Mvc; 
using HomeTrack.Application.Interface;

namespace HomeTrack.Api.Controllers
{
    [ApiController]
    [Route("api/registration")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _regService;

        public RegistrationController(IRegistrationService regService)
        {
            _regService = regService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            await _regService.RegisterAsync(req.Email, req.Password, req.FirstName, req.LastName);
            return Ok("OTP sent to email");
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] SubmitOTPRequest req)
        {
            var result = await _regService.SubmitOTPAsync(req.Token);
            return result ? Ok("User activated") : BadRequest("Invalid or expired token");
        }
    }
}