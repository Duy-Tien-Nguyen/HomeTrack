using HomeTrack.Api.Request;
using HomeTrack.Domain;
using HomeTrack.Domain.Enum;

namespace HomeTrack.Application.Interface
{
  public interface IAdminService
  {
    Task<UserDto> ViewUserDetail(int userId);
    Task<List<UserDto>?> GetAllUser();
    Task<bool> ChangeItemLimit(PackageType packageType, int newLimit);
    Task<bool> UpgradeDowngrade(int userId, PackageType packageType);
    Task<bool> BanUnlock(int userId, UserStatus userStatus);
    Task<ServiceResult<IEnumerable<StatsReport>>> GetSystemLogsAsync(int? userId, string? actionType, DateTime? startTime, DateTime? endTime);
    Task<ServiceResult<IEnumerable<object>>> GetUserRegistrationsPerMonthAsync();
    Task<ServiceResult<IEnumerable<object>>> GetItemCreationsPerMonthAsync();
  }
}