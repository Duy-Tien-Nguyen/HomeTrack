using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Data;
using HomeTrack.Api.Request;
using Microsoft.EntityFrameworkCore;

namespace HomeTrack.Infrastructure.Repositories
{
  public class DashboardRepository : IDashboardRepository
  {
    private readonly ApplicationDBContext _context;

    public DashboardRepository(ApplicationDBContext context)
    {
      _context = context;
    }

    public async Task<IEnumerable<MonthlyCountDto>> GetUserRegistrationsByMonthAsync(int year)
    {
      return await _context.Users // DbSet<User>
          .Where(u => u.CreatedAt.Year == year)
          .GroupBy(u => u.CreatedAt.Month)
          .Select(g => new MonthlyCountDto
          {
            Month = g.Key,
            MonthName = new DateTime(year, g.Key, 1).ToString("MMMM", new System.Globalization.CultureInfo("en-US")), // Specify culture for month name consistency
            Count = g.Count()
          })
          .OrderBy(r => r.Month)
          .ToListAsync();
    }

    public async Task<IEnumerable<MonthlyCountDto>> GetNewItemsByMonthAsync(int year)
    {
      return await _context.Items // DbSet<Item>
          .Where(i => i.CreatedAt.Year == year)
          .GroupBy(i => i.CreatedAt.Month)
          .Select(g => new MonthlyCountDto
          {
            Month = g.Key,
            MonthName = new DateTime(year, g.Key, 1).ToString("MMMM", new System.Globalization.CultureInfo("en-US")),
            Count = g.Count()
          })
          .OrderBy(r => r.Month)
          .ToListAsync();
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
      var totalUsers = await _context.Users.CountAsync();
      var totalItems = await _context.Items.CountAsync();

      return new DashboardSummaryDto
      {
        TotalUsers = totalUsers,
        TotalItems = totalItems
      };
    }
  }
}