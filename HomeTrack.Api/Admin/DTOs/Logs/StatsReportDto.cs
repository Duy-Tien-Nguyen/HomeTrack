using HomeTrack.Domain.Enum;

namespace HomeTrack.Api.Request
{
  public class StatsReportDto
  {
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserEmail { get; set; } // Lấy từ User entity
    public string? UserFullName { get; set; } // Lấy từ User entity (FirstName + LastName)
    public int? ItemId { get; set; }
    public string? ItemName { get; set; } // Lấy từ Item entity
    public ActionType ActionType { get; set; }
    public DateTime Timestamp { get; set; }
  }
}