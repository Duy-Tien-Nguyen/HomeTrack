using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTrack.Domain.Enum; // For Role enum
using Microsoft.Extensions.Logging;

namespace HomeTrack.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<StatisticsService> _logger;

        public StatisticsService(ApplicationDBContext context, ILogger<StatisticsService> logger)
        {
            _context = context;
            _logger = logger;
        }

        private DateTime GetStartDate(string timeframe)
        {
            DateTime startDate;
            switch (timeframe.ToLower())
            {
                case "daily":
                    startDate = DateTime.UtcNow.AddDays(-1);
                    break;
                case "weekly":
                    startDate = DateTime.UtcNow.AddDays(-7);
                    break;
                case "monthly":
                    startDate = DateTime.UtcNow.AddMonths(-1);
                    break;
                default:
                    throw new ArgumentException("Invalid timeframe. Supported values are 'daily', 'weekly', 'monthly'.");
            }
            return startDate;
        }

        private async Task<ServiceResult<bool>> CheckPremiumUser(int userId)
        {
            // Tạm thời bỏ qua kiểm tra người dùng Premium cho mục đích phát triển
            return ServiceResult<bool>.Success(true);

            // Logic gốc (đã comment):
            // var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            // if (user == null)
            // {
            //     return ServiceResult<bool>.Failure("Người dùng không tồn tại.");
            // }
            // if (user.Role != Role.Premium)
            // {
            //     return ServiceResult<bool>.Failure("Tính năng này chỉ dành cho người dùng Premium.");
            // }
            // return ServiceResult<bool>.Success(true);
        }

        public async Task<ServiceResult<UsageStatsResponseDto>> GetItemUsageStatsAsync(string timeframe, int userId)
        {
            try
            {
                var premiumCheck = await CheckPremiumUser(userId);
                if (!premiumCheck.IsSuccess)
                {
                    return ServiceResult<UsageStatsResponseDto>.Failure(premiumCheck.ErrorMessage!);
                }

                DateTime startDate = GetStartDate(timeframe);

                var actionCounts = await _context.StatsReports
                                                 .Where(sr => sr.UserId == userId && sr.Timestamp >= startDate)
                                                 .GroupBy(sr => sr.ActionType)
                                                 .Select(g => new { Action = g.Key, Count = g.Count() })
                                                 .ToDictionaryAsync(x => x.Action.ToString(), x => x.Count);

                var totalLocations = await _context.Locations.Where(l => l.UserId == userId).CountAsync();

                _logger.LogInformation($"Total locations for user {userId}: {totalLocations}");

                var responseDto = new UsageStatsResponseDto
                {
                    ActionCounts = actionCounts,
                    TotalLocations = totalLocations
                };

                return ServiceResult<UsageStatsResponseDto>.Success(responseDto);
            }
            catch (ArgumentException argEx)
            {
                return ServiceResult<UsageStatsResponseDto>.Failure(argEx.Message);
            }
            catch (Exception ex)
            {
                return ServiceResult<UsageStatsResponseDto>.Failure($"Đã có lỗi xảy ra khi lấy thống kê sử dụng: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<RankedItemDto>>> GetTopMovedItemsAsync(string timeframe, int userId)
        {
            try
            {
                var premiumCheck = await CheckPremiumUser(userId);
                if (!premiumCheck.IsSuccess)
                {
                    return ServiceResult<IEnumerable<RankedItemDto>>.Failure(premiumCheck.ErrorMessage!);
                }

                DateTime startDate = GetStartDate(timeframe);

                var topMovedItems = await _context.StatsReports
                                                  .Where(sr => sr.UserId == userId &&
                                                               sr.ActionType == ActionType.Moved &&
                                                               sr.ItemId.HasValue &&
                                                               sr.Timestamp >= startDate)
                                                  .GroupBy(sr => sr.ItemId)
                                                  .Select(g => new { ItemId = g.Key!.Value, Count = g.Count() })
                                                  .OrderByDescending(x => x.Count)
                                                  .Take(5)
                                                  .Join(_context.Items, // Join with Items to get ItemName
                                                        stats => stats.ItemId,
                                                        item => item.Id,
                                                        (stats, item) => new RankedItemDto
                                                        {
                                                            ItemId = stats.ItemId,
                                                            ItemName = item.Name,
                                                            Count = stats.Count
                                                        })
                                                  .ToListAsync();

                return ServiceResult<IEnumerable<RankedItemDto>>.Success(topMovedItems);
            }
            catch (ArgumentException argEx)
            {
                return ServiceResult<IEnumerable<RankedItemDto>>.Failure(argEx.Message);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<RankedItemDto>>.Failure($"Đã có lỗi xảy ra khi lấy top đồ vật được di chuyển: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<RankedItemDto>>> GetLeastUsedItemsAsync(string timeframe, int userId)
        {
            try
            {
                var premiumCheck = await CheckPremiumUser(userId);
                if (!premiumCheck.IsSuccess)
                {
                    return ServiceResult<IEnumerable<RankedItemDto>>.Failure(premiumCheck.ErrorMessage!);
                }

                DateTime startDate = GetStartDate(timeframe);

                // Lấy tất cả các item của người dùng chưa bị xóa
                var userItems = await _context.Items
                                              .Where(item => item.UserId == userId && item.DeletedAt == null)
                                              .ToListAsync();

                // Lấy tất cả các action cho các item của người dùng trong khoảng thời gian
                var itemUsageCounts = await _context.StatsReports
                                                    .Where(sr => sr.UserId == userId &&
                                                                 sr.ItemId.HasValue &&
                                                                 sr.Timestamp >= startDate)
                                                    .GroupBy(sr => sr.ItemId)
                                                    .Select(g => new { ItemId = g.Key!.Value, UsageCount = g.Count() })
                                                    .ToDictionaryAsync(x => x.ItemId, x => x.UsageCount);

                var leastUsedItems = userItems
                    .Select(item => new RankedItemDto
                    {
                        ItemId = item.Id,
                        ItemName = item.Name,
                        Count = itemUsageCounts.GetValueOrDefault(item.Id, 0) // Lấy số lượng sử dụng, mặc định là 0 nếu không có action
                    })
                    .OrderBy(item => item.Count) // Sắp xếp tăng dần theo số lượng sử dụng
                    .Take(5)
                    .ToList();

                return ServiceResult<IEnumerable<RankedItemDto>>.Success(leastUsedItems);
            }
            catch (ArgumentException argEx)
            {
                return ServiceResult<IEnumerable<RankedItemDto>>.Failure(argEx.Message);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<RankedItemDto>>.Failure($"Đã có lỗi xảy ra khi lấy top đồ vật ít được sử dụng: {ex.Message}");
            }
        }
    }
}
