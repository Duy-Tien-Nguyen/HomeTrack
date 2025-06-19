using HomeTrack.Infrastructure.Data;
using HomeTrack.Domain;
using HomeTrack.Application.Interface;
using HomeTrack.Api.Request;
using Microsoft.EntityFrameworkCore;

namespace HomeTrack.Infrastructure.Repositories
{
  public class StatsReportRepository : IStatsReportRepository
  {
    private readonly ApplicationDBContext _context;

    public StatsReportRepository(ApplicationDBContext context)
    {
      _context = context;
    }

    public async Task<PagedResultDto<StatsReportDto>> GetStatsReportsAsync(LogQueryParameters queryParameters)
    {
      var query = _context.StatsReports
                                .Include(sr => sr.User)   // Eager load User
                                .Include(sr => sr.Item)   // Eager load Item
                                .AsQueryable();

      if (queryParameters.UserId.HasValue)
      {
        query = query.Where(sr => sr.UserId == queryParameters.UserId.Value);
      }

      if (queryParameters.ActionType.HasValue)
      {
        query = query.Where(sr => sr.ActionType == queryParameters.ActionType.Value);
      }

      if (queryParameters.StartTime.HasValue)
      {
        query = query.Where(sr => sr.Timestamp >= queryParameters.StartTime.Value);
      }

      if (queryParameters.EndTime.HasValue)
      {
        query = query.Where(sr => sr.Timestamp <= queryParameters.EndTime.Value.Date.AddDays(1).AddTicks(-1));
      }

      query = query.OrderByDescending(sr => sr.Timestamp);

      var totalRecords = await query.CountAsync();

      var reportsData = await query
          .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
          .Take(queryParameters.PageSize)
          .Select(sr => new StatsReportDto
          {
            Id = sr.Id,
            UserId = sr.UserId,
            UserEmail = sr.User.Email,
            UserFullName = sr.User.FirstName != null && sr.User.LastName != null
                                    ? $"{sr.User.FirstName} {sr.User.LastName}"
                                    : (sr.User.FirstName ?? sr.User.LastName ?? sr.User.Email), // Fallback logic
            ItemId = sr.ItemId,
            ItemName = sr.Item != null ? sr.Item.Name : null,
            ActionType = sr.ActionType,
            Timestamp = sr.Timestamp
          })
          .ToListAsync();

      return new PagedResultDto<StatsReportDto>
      {
        TotalRecords = totalRecords,
        PageSize = queryParameters.PageSize,
        CurrentPage = queryParameters.PageNumber,
        TotalPages = (int)Math.Ceiling(totalRecords / (double)queryParameters.PageSize),
        Data = reportsData
      };
    }

    public async Task AddReportAsync(StatsReport report)
    {
      await _context.StatsReports.AddAsync(report);
      await _context.SaveChangesAsync();
    }
  }
}