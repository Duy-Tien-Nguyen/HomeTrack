using System;
using System.Collections.Generic; 

namespace HomeTrack.Api.Models.DTOs
{
    public class LocationResponseDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? ParentLocationId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}
