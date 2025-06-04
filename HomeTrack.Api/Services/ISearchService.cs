using HomeTrack.Api.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTrack.Api.Services
{
    public interface ISearchService
    {
        Task<ServiceResult<IEnumerable<ItemViewModel>>> BasicSearchItemsAsync(string keyword, int userId);
        Task<ServiceResult<IEnumerable<ItemViewModel>>> AdvancedSearchItemsAsync(AdvancedSearchRequestDto dto, int userId);
    }
} 