using HomeTrack.Domain.Enum;

namespace HomeTrack.Api.Request
{
  public class UserCountByRoleDto
  {
    public Role Role { get; set; }
    public string RoleName => Role.ToString(); // Trả về tên của enum dưới dạng string
    public int Count { get; set; }
  }
}