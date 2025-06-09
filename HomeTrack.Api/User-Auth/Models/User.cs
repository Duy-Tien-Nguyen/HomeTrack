using HomeTrack.Domain.Account;
using HomeTrack.Domain.Enum;

namespace HomeTrack.Domain
{
  public class User
  {
    public int Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required Role Role { get; set; }
    public string? RefreshToken { get; set; }
    public UserStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<ConfirmationToken> ConfirmationTokens { get; set; } = new List<ConfirmationToken>();
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public virtual ICollection<FileStorage> FileStorages { get; set; } = new List<FileStorage>();
    public User()
    {
      CreatedAt = DateTime.UtcNow;
      UpdatedAt = DateTime.UtcNow;
      Status = UserStatus.Pending; // Default status
      Role = Role.Basic; // Default role
    }
    public bool ValidatePassword(string password)
    {
      // Implement password validation logic here
      // For example, check if the password matches the stored hash
      return Password == password; // Simplified for demonstration purposes
    }
    public void Active()
    {
      Status = UserStatus.Active;
      UpdatedAt = DateTime.UtcNow;
    }
  }
}