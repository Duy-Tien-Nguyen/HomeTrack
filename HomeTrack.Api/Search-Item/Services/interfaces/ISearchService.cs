using HomeTrack.Api.Request;

namespace HomeTrack.Application.Interface
{
    public interface ISearchService
    {
        Task<ServiceResult<IEnumerable<ItemViewModel>>> BasicSearchItemsAsync(string keyword, int userId);
        Task<ServiceResult<IEnumerable<ItemViewModel>>> AdvancedSearchItemsAsync(AdvancedSearchRequestDto dto, int userId);
    }
} 