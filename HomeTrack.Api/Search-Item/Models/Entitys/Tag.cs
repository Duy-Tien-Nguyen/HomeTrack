using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HomeTrack.Domain.Enum;

namespace HomeTrack.Domain
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên tag là bắt buộc")]
        [StringLength(100)]
        public required string Name { get; set; }

        public int? UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public TagModerationStatus ModerationStatus { get; set; } = TagModerationStatus.Pending;
        public virtual ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
    }
}