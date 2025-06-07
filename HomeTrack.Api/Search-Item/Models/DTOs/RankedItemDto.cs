namespace HomeTrack.Api.Request
{
    public class RankedItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
