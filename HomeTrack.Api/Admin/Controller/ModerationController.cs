using HomeTrack.Application.Interface;
using HomeTrack.Api.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeTrack.Domain.Enum;
using System.Security.Claims;

namespace HomeTrack.Api.Controllers
{
  [Authorize(Roles = "Admin")]
  [Route("api/[controller]")]
  [ApiController]
  public class ModerationController : ControllerBase
  {
    private readonly IModerationRepository _moderationRepository;
    public ModerationController(IModerationRepository moderationRepository)
    {
      _moderationRepository = moderationRepository ?? throw new ArgumentNullException(nameof(moderationRepository));
    }


    [HttpGet("pending-items")]
    [ProducesResponseType(typeof(PagedResultDto<PendingItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPendingItems([FromQuery] PagedQueryParameters queryParameters)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }
      var result = await _moderationRepository.GetPendingItemsAsync(queryParameters);
      return Ok(result);
    }

    [HttpGet("items/{itemId}")]
    [ProducesResponseType(typeof(PendingItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetItemForModeration(int itemId)
    {
      var item = await _moderationRepository.GetItemByIdForModerationAsync(itemId);
      if (item == null)
      {
        return NotFound($"Không timg thấy Item với ID {itemId}");
      }

      var itemDto = new PendingItemDto
      {
        Id = item.Id,
        Name = item.Name,
        ImageUrl = item.ImageUrl,
        UserId = item.UserId,
        UserEmail = item.User?.Email ?? "N/A",
        CreatedAt = item.CreatedAt,
        ImageModerationStatus = item.ImageModerationStatus,
        ItemModerationNoteFromAI = item.ModerationNote,
        AssociatedTags = item.ItemTags.Select(it => new ItemTagViewModel
        {
          TagId = it.TagId,
          TagName = it.Tag.Name,
          TagModerationStatus = it.Tag.ModerationStatus
        }).ToList()
      };
      return Ok(itemDto);
    }

    [HttpPut("item/{itemId}/moderate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ModerateItem(int itemId, [FromBody] ModerateItemRequestDto request)
    {
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      if (request.NewStatus != ModerationStatus.Approved && request.NewStatus != ModerationStatus.Reject)
      {
        return BadRequest("Trạng thái kiểm duyệt Item không hợp lệ. Chỉ chấp nhận 'Approved' hoặc 'Rejected'.");
      }

      var itemToModerate = await _moderationRepository
                                .GetItemByIdForModerationAsync(itemId);
      if (itemToModerate == null)
      {
        return NotFound($"Không tìm thấy Item với ID {itemId} để kiểm duyệt.");
      }

      if (itemToModerate.ImageModerationStatus != ModerationStatus.Pending)
      {
        Console.WriteLine($"Admin is re-moderating item {itemId} which was already {itemToModerate.ImageModerationStatus}");
      }

      var adminUserIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (string.IsNullOrEmpty(adminUserIdClaim) || !int.TryParse(adminUserIdClaim, out int adminUserId))
      {
        return Unauthorized("Không thể xác định Admin thực hiện hành động.");
      }

      itemToModerate.ImageModerationStatus = request.NewStatus;
      itemToModerate.ModerationNote = request.NewStatus == ModerationStatus.Reject
                                      ? $"Admin Rejected: {request.ModerationNotes}"
                                      : $"Admin Approved. {(string.IsNullOrEmpty(request.ModerationNotes) ? "" : "Notes: " + request.ModerationNotes)}";
      itemToModerate.ModeratedAt = DateTime.UtcNow;
      itemToModerate.ModerationByUserId = adminUserId;

      if (itemToModerate.ImageModerationStatus == ModerationStatus.Approved)
      {
        foreach (var itemTag in itemToModerate.ItemTags)
        {
          if (itemTag.Tag.ModerationStatus == TagModerationStatus.Pending)
          {
            itemTag.Tag.ModerationStatus = TagModerationStatus.Approved;
          }
        }
      }

      var success = await _moderationRepository.UpdateItemAsync(itemToModerate);

      if (success)
      {
        return Ok("Cập nhật thành công");
      }

      return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi xảy ra khi cập nhật trạng thái kiểm duyệt Item.");
    }
  }
}