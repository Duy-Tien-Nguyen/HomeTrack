using HomeTrack.Api.Request; 

namespace HomeTrack.Application.Interface
{
    public interface ILocationService
    {
        Task<ServiceResult<LocationResponseDto>> CreateLocationAsync(LocationCreateRequestDto locationDto, int userId);

        Task<ServiceResult<IEnumerable<LocationResponseDto>>> GetLocationsAsync(int userId);

        Task<ServiceResult<LocationResponseDto>> GetLocationByIdAsync(int locationId, int userId);

        Task<ServiceResult<LocationResponseDto>> UpdateLocationAsync(int locationId, LocationUpdateRequestDto locationDto, int userId);

        Task<ServiceResult<bool>> DeleteLocationAsync(int locationId, int userId);
    }
}
