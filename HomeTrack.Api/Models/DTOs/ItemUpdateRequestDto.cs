using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace HomeTrack.Api.Models.DTOs
{
    public class ItemUpdateRequestDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<string>? Tags { get; set; }
        public int? LocationId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Color { get; set; }
    }
}
