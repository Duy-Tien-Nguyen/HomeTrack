using HomeTrack.Application.Services;

namespace HomeTrack.Api.Request
{
    public class LocationResponseDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? ParentLocationId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public ICollection<ItemViewModel>? Items { get; set; }
    }
}
