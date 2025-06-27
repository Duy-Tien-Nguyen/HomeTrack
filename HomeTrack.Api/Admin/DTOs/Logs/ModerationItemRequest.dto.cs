using HomeTrack.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace HomeTrack.Api.Request
{
  public class ModerateItemRequestDto
  {
    [Required(ErrorMessage = "Trạng thái kiểm duyệt mới là bắt buộc.")]
    [EnumDataType(typeof(ModerationStatus), ErrorMessage = "Giá trị trạng thái kiểm duyệt không hợp lệ.")]
    public ModerationStatus NewStatus { get; set; }
    [StringLength(1000, ErrorMessage = "Ghi chú kiểm duyệt không được vượt quá 1000 ký tự.")]
    public string? ModerationNotes { get; set; }
  }

  public class ModerateTagRequestDto
  {
    [Required(ErrorMessage = "Trạng thái kiểm duyệt mới cho Tag là bắt buộc.")]
    [EnumDataType(typeof(TagModerationStatus), ErrorMessage = "Giá trị trạng thái kiểm duyệt Tag không hợp lệ.")]
    public TagModerationStatus NewStatus { get; set; } // Approved hoặc Rejected

    // Bạn có thể thêm ModerationNotes cho Tag nếu cần
    [StringLength(500, ErrorMessage = "Ghi chú kiểm duyệt Tag không được vượt quá 500 ký tự.")]
    public string? ModerationNotes { get; set; }
  }
}