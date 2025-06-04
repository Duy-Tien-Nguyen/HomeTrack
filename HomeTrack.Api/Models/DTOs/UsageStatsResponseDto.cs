using System.Collections.Generic;

namespace HomeTrack.Api.Models.DTOs
{
    public class UsageStatsResponseDto
    {
        public Dictionary<string, int> ActionCounts { get; set; } = new Dictionary<string, int>();
    }
}
