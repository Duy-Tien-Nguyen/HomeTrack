namespace HomeTrack.Api.Request
{
    public class UsageStatsResponseDto
    {
        public Dictionary<string, int> ActionCounts { get; set; } = new Dictionary<string, int>();
        public int TotalLocations { get; set; }
    }
}
