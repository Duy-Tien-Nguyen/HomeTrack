using Microsoft.AspNetCore.Mvc;
using HomeTrack.Application.Interface;
using HomeTrack.Api.Request;

namespace HomeTrack.Api.Controllers
{
  [ApiController]
  [Route("api/registration")]
  public class RegistrationController : ControllerBase
  {
    private readonly IRegistrationService _regService;
    private readonly ILogger<RegistrationController> _logger;

    public RegistrationController(IRegistrationService regService, ILogger<RegistrationController> logger)
    {
      _regService = regService;
      _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
      try
      {
        await _regService.RegisterAsync(req.Email, req.Password, req.FirstName, req.LastName);
        return Ok(new { message = "OTP đã được gửi đến email." });
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi đăng ký tài khoản cho email {Email}", req.Email);
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đăng ký thất bại." });
      }
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] SubmitOTPRequest req)
    {
      try
      {
        var result = await _regService.VerifyOTP(req.Email, req.Token);
        if (result)
        {
          return Ok(new VerifyOTPRespone { status = true });
        }
        else
        {
          return BadRequest(new VerifyOTPRespone { status = false });
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi xác thực OTP cho email {Email}", req.Email);
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Xác thực OTP thất bại." });
      }
    }

    [HttpPost("resendOTP")]
    public async Task<IActionResult> ResendOTP([FromBody] ResendOTPRequest req)
    {
      try
      {
        var result = await _regService.ResendOTP(req.email);
        if (result)
        {
          return Ok(new { message = "OTP đã được gửi lại đến email." });
        }
        else
        {
          return BadRequest(new { message = "Dịch vụ đang bảo trì hoặc email không hợp lệ." });
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Lỗi khi gửi lại OTP cho email {Email}", req.email);
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Không thể gửi lại OTP." });
      }
    }
  }
}
