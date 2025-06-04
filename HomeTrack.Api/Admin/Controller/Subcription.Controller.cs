using System.Security.Claims;
using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTrack.Api.Controllers
{
  [Route("api/subscriptions")]
  [ApiController]
  public class SubscriptionController : ControllerBase
  {
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPackageService _packageService;
    private readonly IUserRepository _userRepository;
    public SubscriptionController(
      ISubscriptionService subscriptionService,
      IPackageService packageService,
      IUserRepository userRepository)
    {
      _subscriptionService = subscriptionService;
      _packageService = packageService;
      _userRepository = userRepository;
    }

    [HttpGet("byid")]
    [Authorize]
    public async Task<IActionResult> GetSubscriptionById([FromBody] GetSubscriptionByIdReq req)
    {
      var subscription = await _subscriptionService.GetByIdAsync(req.subcriptionId);
      if (subscription == null)
      {
        return NotFound();
      }
      return Ok(subscription);
    }

    [HttpGet("all")]
    [Authorize(Roles ="Admin")]
    public async Task<IActionResult> GetAllSubscriptions()
    {
      var subscriptions = await _subscriptionService.GetAllAsync();
      if (subscriptions == null || !subscriptions.Any())
      {
        return NotFound();
      }
      return Ok(subscriptions);
    }

    [HttpPost("regis-subcription")]
    [Authorize]
    public async Task<IActionResult> RegisAsync([FromBody] CreateSubscriptionDto createSubscriptionDto)
    {
      var subscription = await _subscriptionService.AddAsync(createSubscriptionDto);
      if (subscription == null)
      {
        return StatusCode(500, "Lỗi xuất hiện khi đăng ký gói.");
      }
      return Ok(subscription);
    }

    [HttpPost("cancel-subs")]
    [Authorize]
    public async Task<IActionResult> CancelSubs([FromBody] GetSubscriptionByIdReq req)
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (!int.TryParse(userIdClaim, out var userId))
        return Unauthorized("ID người dùng không hợp lệ");

      var result = await _subscriptionService.CancelAsync(req.subcriptionId, userId);

      if (result == null)
      {
        return StatusCode(500, "Hủy gói thất bại");
      }
      return Ok(result);
    }

    [HttpPost("subs-expire")]
    [Authorize(Roles ="Admin")]
    public async Task<IActionResult> ExpireAsync([FromBody] ExpireSubscriptionReq req)
    {
      var result = await _subscriptionService.ExpireAsync(req.subscriptionId, req.userId);

      if (result == null)
      {
        return StatusCode(500, "Hủy gói thất bại");
      }
      return Ok(result);
    }
  }
}