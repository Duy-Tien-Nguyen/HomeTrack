namespace HomeTrack.Api.Request
{
  public class MonthlyCountDto
  {
    public int Month { get; set; }
    public string MonthName { get; set; }
    public int Count { get; set; }
  }
  
  public class DashboardSummaryDto
  {
    public int TotalUsers { get; set; }
    public int TotalItems { get; set; }
  }
}