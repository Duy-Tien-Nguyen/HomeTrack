using System.ComponentModel.DataAnnotations;

namespace HomeTrack.Api.Request
{ 
    public class CreateItemDto
    {
        [Required(ErrorMessage = "Tên đồ vật là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên đồ vật không được vượt quá 100 ký tự.")]
        public required string Name { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }

        public List<string>? Tags { get; set; }

        public int? LocationId { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? Color { get; set; }
    }
}