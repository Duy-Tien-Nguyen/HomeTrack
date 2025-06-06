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
    public User()
    {
      CreatedAt = DateTime.UtcNow;
      UpdatedAt = DateTime.UtcNow;
      Status = UserStatus.PendingVerification; 
      Role = Role.Basic;
    }
    public bool ValidatePassword(string password)
    {
  
      return Password == password; 
    }
    public void Active()
    {
      Status = UserStatus.Active;
      UpdatedAt = DateTime.UtcNow;
    }
  }
}