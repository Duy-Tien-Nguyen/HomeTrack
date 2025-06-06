using System.Collections.Generic;

namespace HomeTrack.Api.Models.DTOs
{
    public class AdvancedSearchRequestDto
    {
        public List<string>? Tags { get; set; }
        public string? Color { get; set; }
        public string? SortBy { get; set; } // e.g., "mru" (most recently used), "lru" (least recently used)
    }
}