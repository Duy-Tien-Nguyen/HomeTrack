using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HomeTrack.Api.Models.Entities
{
public class FileStorage
{
    [Key]
    public required Guid Id { get; set; }

    [Required]
    [StringLength(255)] 
    public required string FileName { get; set; }

    [Required]
    [StringLength(1024)] 
    public required string FilePath { get; set; }

    [StringLength(100)] // VARCHAR NULL
    public string? ContentType { get; set; }

    public long? FileSize { get; set; }

    public int? UserId { get; set; } 

    public int? ItemId { get; set; } 

    [Required]
    public required DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("UserId")]
    public virtual User? User { get; set; } 

    [ForeignKey("ItemId")]
    public virtual Item? Item { get; set; } 
}
}