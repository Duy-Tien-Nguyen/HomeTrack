namespace HomeTrack.Domain
{

public class Package
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public string? Description { get; set; }
  public decimal Price { get; set; }
  public int DurationDays { get; set; }
  public bool isActive { get; set; }
  public DateTime CreateAt { get; set; }
  public DateTime UpdateAt { get; set; }

  // Navigation property - Đảm bảo Subscription Entity cũng dùng chung namespace nếu có thể
  public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
}