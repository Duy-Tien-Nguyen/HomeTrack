using HomeTrack.Api.Request;

namespace HomeTrack.Api.Request
{
  public class SubscriptionDto
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;

    public int PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;

    public SubscriptionStatus Status { get; set; }
    public string StatusText => Status.ToString();
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

  public class GetSubscriptionByIdReq
  {
    public required int subcriptionId { get; set; }
  }

  public class ExpireSubscriptionReq
  {
    public required int subscriptionId { get; set; }
    public required int userId{ get; set; }
  }

}