using System;
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema; 
using HomeTrack.Domain.Enum;


namespace HomeTrack.Api.Models.Entities
{
public class User
{
    [Key] 
    public int Id { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc")] 
    [StringLength(255)] 
    [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
    public required string Email { get; set; }

    [StringLength(255)] 
    public required string Password { get; set; }

    [StringLength(100)]
    public string? Firstname { get; set; } 

    [StringLength(100)]
    public string? Lastname { get; set; } 

    public required Role Role { get; set; }

    public required UserStatus Status { get; set; }

    public string? RefreshToken { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<ConfirmationToken> ConfirmationTokens { get; set; } = new List<ConfirmationToken>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<FileStorage> FileStorages { get; set; } = new List<FileStorage>();
}
}