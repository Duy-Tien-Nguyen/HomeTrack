using HomeTrack.Domain.Enum;

namespace HomeTrack.Api.Request
{
  public class LogQueryParameters
  {
    public int? UserId { get; set; } // Id của User là int
    public ActionType? ActionType { get; set; } // Sử dụng enum ActionType
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
  }
}