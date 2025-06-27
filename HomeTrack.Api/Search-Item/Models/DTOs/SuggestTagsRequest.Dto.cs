using System.ComponentModel.DataAnnotations;

namespace HomeTrack.Api.Request
{
  public class SuggestTagsRequestDto
  {
    [Required(ErrorMessage = "File ảnh là bắt buộc.")]
    public required IFormFile ImageFile { get; set; }

    public string? ItemContextText { get; set; }
  }
}