using System;
using System.Collections.Generic; 

namespace HomeTrack.Api.Models.DTOs
{
    public class ItemViewModel
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? LocationId { get; set; }
        public List<string>? Tags { get; set; }
        public required DateTime CreatedAt { get; set; }
        public string? Color { get; set; }
    }
}
