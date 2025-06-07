using System.ComponentModel.DataAnnotations; 

namespace HomeTrack.Api.Request
{
    public class LocationUpdateRequestDto
    {
        [StringLength(255, ErrorMessage = "Tên vị trí không được vượt quá 255 ký tự.")]
        public string? Name { get; set; }

        public int? ParentLocationId { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? Description { get; set; }
    }
}
