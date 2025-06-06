using HomeTrack.Api.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTrack.Api.Services
{
    public interface IStatisticsService
    {
        Task<ServiceResult<UsageStatsResponseDto>> GetItemUsageStatsAsync(string timeframe, int userId);
        Task<ServiceResult<IEnumerable<RankedItemDto>>> GetTopMovedItemsAsync(string timeframe, int userId);
        Task<ServiceResult<IEnumerable<RankedItemDto>>> GetLeastUsedItemsAsync(string timeframe, int userId);
    }
}
