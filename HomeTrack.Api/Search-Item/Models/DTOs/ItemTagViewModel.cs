using HomeTrack.Domain.Enum;

namespace HomeTrack.Api.Request
{
  public class ItemTagViewModel
  {
    public int TagId { get; set; }
    public required string TagName { get; set; }
    public TagModerationStatus TagModerationStatus { get; set; } // Trạng thái của chính Tag đó
  }

  public class PendingItemDto
  {
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; }
    public DateTime CreatedAt { get; set; }
    public ModerationStatus ImageModerationStatus { get; set; } // Sẽ là Pending cho Item này
    public string? ItemModerationNoteFromAI { get; set; } // Ghi chú từ AI cho Item
    public List<ItemTagViewModel> AssociatedTags { get; set; } = new List<ItemTagViewModel>();
  }
}