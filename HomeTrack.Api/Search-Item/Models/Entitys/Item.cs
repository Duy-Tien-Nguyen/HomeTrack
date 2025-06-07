using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTrack.Domain
{
[Table("items")]
public class Item
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; } 
    [ForeignKey("UserId")] 
    public required virtual User User { get; set; }

    [Required(ErrorMessage = "Tên đồ vật là bắt buộc")] 
    [StringLength(255)]
    public required string Name { get; set; }

    [Column(TypeName = "TEXT")] 
    public string? Description { get; set; }

    [StringLength(2048)] // URL có thể khá dài
    public string? ImageUrl { get; set; }

    public string? Color { get; set; } // Thuộc tính Color của đồ vật

    public int? LocationId { get; set; } 
    [ForeignKey("LocationId")]
    public virtual Location? Location { get; set; } 

    public DateTime? DeletedAt { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<ItemTag> ItemTags { get; set; } = new List<ItemTag>();

    public virtual ICollection<FileStorage> AttachedFiles { get; set; } = new List<FileStorage>();
}
}