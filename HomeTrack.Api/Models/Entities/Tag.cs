using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTrack.Api.Models.Entities
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

    public virtual ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();
}
}