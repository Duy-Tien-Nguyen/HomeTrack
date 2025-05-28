namespace HomeTrack.Domain.Account
{
  public class ConfirmationToken
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Token { get; set; }
    public DateTime ExpirationAt { get; set; }
    public bool Used { get; set; }

    public ConfirmationToken()
    {
      Used = false; // Default value for Used
      ExpirationAt = DateTime.UtcNow.AddMinutes(5); // Default expiration time set to 1 day from creation
    }
  }
}