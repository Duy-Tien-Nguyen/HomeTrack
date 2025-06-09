using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Domain.Enum;
using HomeTrack.Infrastructure.Data;

namespace HomeTrack.Application.Services
{
  public class AdminService : IAdminService
  {
    private readonly ISystemSettingService _settingService;
    private readonly ILogger<AdminService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDBContext _context;
    private static readonly Dictionary<PackageType, string> PackageTypeLimits = new()
    {
      { PackageType.Basic, "MaxBasicItemLimit" },
      { PackageType.Premium, "MaxPremiumItemLimit" }
    };

    private static readonly Dictionary<PackageType, Role> PackageTypeRoles = new()
    {
      { PackageType.Basic, Role.Basic },
      { PackageType.Premium, Role.Premium }
    };
    public AdminService(ISystemSettingService settingService, ILogger<AdminService> logger, IUserRepository userRepository,
      ApplicationDBContext context)
    {
      _context = context;
      _userRepository = userRepository;
      _settingService = settingService;
      _logger = logger;
    }

    public async Task<UserDto> ViewUserDetail(int userId)
    {
      var user = await _userRepository.GetByIdAsync(userId);
      if (user == null)
      {
        _logger.LogWarning($"User with ID {userId} not found.");
        return null;
      }
      return new UserDto
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Role = user.Role.ToString(),
        Email = user.Email,
      };
    }

    public async Task<List<UserDto>?> GetAllUser()
    {
      var users = await _userRepository.GetAllAsync();
      if (users == null || !users.Any())
      {
        _logger.LogWarning("No users found.");
        return null;
      }
      return users.Select(user => new UserDto
      {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Role = user.Role.ToString(),
        Email = user.Email,
      }).ToList();
    }

    public async Task<bool> ChangeItemLimit(PackageType packageType, int newLimit)
    {
      try
      {
        if (newLimit <= 0)
        {
          _logger.LogWarning("New item limit must be greater than zero.");
          return false;
        }

        if (!Enum.IsDefined(typeof(PackageType), packageType))
        {
          _logger.LogWarning("Invalid package type: {PackageType}", packageType);
          return false;
        }
        if (PackageTypeLimits.TryGetValue(packageType, out string settingKey))
        {
          if (string.IsNullOrEmpty(settingKey))
          {
            _logger.LogWarning("Setting key for package type {PackageType} is not defined.", packageType);
            return false;
          }

          var currentLimit = await _settingService.GetByKeyAsync(settingKey);
          if (currentLimit == null)
          {
            _logger.LogWarning("Current limit for package type {PackageType} not found.", packageType);
            return false;
          }

          currentLimit.SettingValue = newLimit;
          await _context.SaveChangesAsync();
          _logger.LogInformation("Successfully changed item limit to {NewLimit} for package type {PackageType} (Setting Key: {SettingKey}).", newLimit, packageType, settingKey);

          return true;
        }

        return false;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to change item limit for package type {PackageType}", packageType);
        return false;
      }
    }

    public async Task<bool> UpgradeDowngrade(int userId, PackageType packageType)
    {
      try
      {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
          _logger.LogWarning("User with ID {UserId} not found.", userId);
          return false;
        }

        if (!Enum.IsDefined(typeof(PackageType), packageType))
        {
          _logger.LogWarning("Invalid package type: {PackageType}", packageType);
          return false;
        }

        if (PackageTypeRoles.TryGetValue(packageType, out Role newRole))
        {
          user.Role = newRole;
          await _userRepository.SaveChangesAsync();
          _logger.LogInformation("Successfully upgraded/downgraded user with ID {UserId} to package type {PackageType} (New Role: {NewRole}).", userId, packageType, newRole);
          return true;
        }

        return false;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to upgrade/downgrade user with ID {UserId} to package type {PackageType}", userId, packageType);
        return false;
      }
    }

    public async Task<bool> BanUnlock(int userId, UserStatus userStatus)
    {
      try
      {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
          _logger.LogWarning("User with ID {UserId} not found.", userId);
          return false;
        }

        user.Status = userStatus;
        await _userRepository.SaveChangesAsync();
        _logger.LogInformation("Successfully updated user status for user with ID {UserId} to {UserStatus}.", userId, userStatus);
        return true;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Failed to update user status for user with ID {UserId} to {UserStatus}", userId, userStatus);
        return false;
      }
    }
  }
}