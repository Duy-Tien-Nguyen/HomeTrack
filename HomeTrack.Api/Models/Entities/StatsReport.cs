using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HomeTrack.Api.Common.Enum; // Đảm bảo import enum ActionType

namespace HomeTrack.Api.Models.Entities
{
    [Table("stats_reports")]
    public class StatsReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        public int? ItemId { get; set; } // Nullable nếu action không liên quan đến item cụ thể

        [ForeignKey("ItemId")]
        public virtual Item? Item { get; set; }

        [Required]
        public ActionType ActionType { get; set; } // Enum: created, edited, moved, searched

        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
