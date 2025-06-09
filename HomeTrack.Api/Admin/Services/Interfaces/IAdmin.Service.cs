using HomeTrack.Api.Request;
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
  }
}