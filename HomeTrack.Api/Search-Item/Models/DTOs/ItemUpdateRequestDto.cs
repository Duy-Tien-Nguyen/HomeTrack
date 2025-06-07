namespace HomeTrack.Api.Request
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
