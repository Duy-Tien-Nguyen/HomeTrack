using HomeTrack.Api.Request;

namespace HomeTrack.Application.Interface
{
  public interface IDashboardRepository
  {
    Task<IEnumerable<MonthlyCountDto>> GetUserRegistrationsByMonthAsync(int year);
    Task<IEnumerable<MonthlyCountDto>> GetNewItemsByMonthAsync(int year);
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
  }
}