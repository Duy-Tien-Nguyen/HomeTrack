using HomeTrack.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTrack.Domain
{
public class Location
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public required virtual User User { get; set; }

    [Required]
    [StringLength(255)] 
    public required string Name { get; set; }

    public int? ParentLocationId { get; set; }

    [Column(TypeName = "TEXT")]
    public string? Description { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [ForeignKey("ParentLocationId")]
    public virtual Location? ParentLocation { get; set; }
    public virtual ICollection<Location> ChildLocations { get; set; } = new List<Location>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
}