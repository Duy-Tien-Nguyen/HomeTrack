using HomeTrack.Api.Request;

namespace HomeTrack.Application.Interface
{
    public interface IStatisticsService
    {
        Task<ServiceResult<UsageStatsResponseDto>> GetItemUsageStatsAsync(string timeframe, int userId);
        Task<ServiceResult<IEnumerable<RankedItemDto>>> GetTopMovedItemsAsync(string timeframe, int userId);
        Task<ServiceResult<IEnumerable<RankedItemDto>>> GetLeastUsedItemsAsync(string timeframe, int userId);
    }
}
