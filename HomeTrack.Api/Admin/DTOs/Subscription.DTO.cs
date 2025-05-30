using HomeTrack.Api.Request;

namespace HomeTrack.Api.Request
{
  public class SubscriptionDto
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserDto? User { get; set; } // Optional: include user details

    public int PackageId { get; set; }
    public PackageDto? Package { get; set; } // Optional: include package details

    public SubscriptionStatus Status { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
  }

  public class CreateSubscriptionDto
  {
    public required int UserId { get; set; }
    public required int PackageId { get; set; }
  }

  public class UpdateSubscriptionStatusDto
  {
    public required SubscriptionStatus Status { get; set; }
  }
}