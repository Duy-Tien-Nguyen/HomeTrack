using HomeTrack.Api.Request;
using HomeTrack.Domain;

namespace HomeTrack.Application.Interface
{
  public interface IModerationRepository
  {
    Task<PagedResultDto<PendingItemDto>> GetPendingItemsAsync(PagedQueryParameters queryParameters);
    Task<Item?> GetItemByIdForModerationAsync(int itemId);
    Task<bool> UpdateItemAsync(Item itemToUpdate);
  }
}
