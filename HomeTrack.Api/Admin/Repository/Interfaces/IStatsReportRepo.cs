using HomeTrack.Api.Request;
using HomeTrack.Domain;

namespace HomeTrack.Application.Interface
{
  public interface IStatsReportRepository
  {
    Task<PagedResultDto<StatsReportDto>> GetStatsReportsAsync(LogQueryParameters queryParameters);
    Task AddReportAsync(StatsReport report); // Để ghi report/log
  }
}