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
      try
      {
        var subscription = await _subscriptionService.GetByIdAsync(req.subcriptionId);
        if (subscription == null)
          return NotFound("Không tìm thấy gói đăng ký");
        return Ok(subscription);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi lấy thông tin: {ex.Message}");
      }
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllSubscriptions()
    {
      try
      {
        var subscriptions = await _subscriptionService.GetAllAsync();
        if (subscriptions == null || !subscriptions.Any())
          return NotFound("Không có gói đăng ký nào");
        return Ok(subscriptions);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi lấy danh sách: {ex.Message}");
      }
    }

    [HttpPost("regis-subcription")]
    [Authorize]
    public async Task<IActionResult> RegisAsync([FromBody] CreateSubscriptionDto dto)
    {
      try
      {
        var subscription = await _subscriptionService.AddAsync(dto);
        if (subscription == null)
          return StatusCode(500, "Lỗi xuất hiện khi đăng ký gói.");
        return Ok(subscription);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi đăng ký: {ex.Message}");
      }
    }

    [HttpGet("user-by-admin")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByUserId([FromQuery] int userId)
    {
      try
      {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
          return NotFound("Người dùng không tồn tại");

        var subscriptions = await _subscriptionService.GetByUserIdAsync(userId);
        if (subscriptions == null || !subscriptions.Any())
          return NotFound("Không có gói đăng ký nào cho người dùng này");

        return Ok(subscriptions);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi xảy ra: {ex.Message}");
      }
    }

    [HttpGet("by-myself")]
    [Authorize]
    public async Task<IActionResult> GetByMyself()
    {
      try
      {
        var userId = GetCurrentUserId();
        if (userId == null)
          return Unauthorized("ID người dùng không hợp lệ");

        var subscriptions = await _subscriptionService.GetByUserIdAsync(userId.Value);
        if (subscriptions == null || !subscriptions.Any())
          return NotFound("Không có gói đăng ký nào");

        return Ok(subscriptions);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi xảy ra: {ex.Message}");
      }
    }

    [HttpPost("cancel-subs")]
    [Authorize]
    public async Task<IActionResult> CancelSubs([FromBody] GetSubscriptionByIdReq req)
    {
      try
      {
        var userId = GetCurrentUserId();
        if (userId == null)
          return Unauthorized("ID người dùng không hợp lệ");

        var result = await _subscriptionService.CancelAsync(req.subcriptionId, userId.Value);
        if (result == null)
          return StatusCode(500, "Hủy gói thất bại");

        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi hủy gói: {ex.Message}");
      }
    }

    [HttpPost("subs-expire")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExpireAsync([FromBody] ExpireSubscriptionReq req)
    {
      try
      {
        var result = await _subscriptionService.ExpireAsync(req.subscriptionId, req.userId);
        if (result == null)
          return StatusCode(500, "Hủy gói thất bại");
        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi hủy gói: {ex.Message}");
      }
    }

    [HttpPost("subs-active")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ActivateAsync([FromBody] ExpireSubscriptionReq req)
    {
      try
      {
        var result = await _subscriptionService.ActivateAsync(req.subscriptionId, req.userId);
        if (!result)
          return StatusCode(500, "Kích hoạt gói thất bại");
        return Ok(result);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi kích hoạt gói: {ex.Message}");
      }
    }

    private int? GetCurrentUserId()
    {
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      return int.TryParse(userIdClaim, out var userId) ? userId : (int?)null;
    }
  }
}