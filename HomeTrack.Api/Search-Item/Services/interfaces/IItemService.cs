using HomeTrack.Api.Request;

namespace HomeTrack.Application.Interface
{
public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }

    public static ServiceResult<T> Success(T data) => new ServiceResult<T> { IsSuccess = true, Data = data };
    public static ServiceResult<T> Failure(string errorMessage) => new ServiceResult<T> { IsSuccess = false, ErrorMessage = errorMessage };
}

public interface IItemService
{
    Task<ServiceResult<ItemViewModel>> CreateNewItem(CreateItemDto itemDto, int userId);

    Task<ServiceResult<ItemViewModel>> GetItemByIdAsync(int itemId, int userId);

    Task<ServiceResult<ItemViewModel>> UpdateItemAsync(int itemId, ItemUpdateRequestDto itemDto, int userId);

    Task<ServiceResult<bool>> DeleteItemAsync(int itemId, int userId);

    Task<ServiceResult<IEnumerable<ItemViewModel>>> GetItemsByLocationAsync(int locationId, int userId);

    Task<ServiceResult<IEnumerable<ItemViewModel>>> GetAllItemsAsync(int userId);
}
}
