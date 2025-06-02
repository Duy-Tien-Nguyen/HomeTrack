namespace HomeTrack.Api.Request

{
  public class LoginRequest
  {
    public required string email { get; set; }
    public required string password { get; set; }
  }

  // DTO cho kết quả đăng nhập thành công
  public class LoginResponseDto
  {
    public bool IsSuccess { get; set; }
    public string? AccessToken { get; set; } //Token để truy cập API
    public string? RefreshToken { get; set; } //Token để làm mới AccessToken
    public string? ErrorMessage { get; set; } //Thông báo lỗi nếu đăng nhập thất bại
    public UserDto? User { get; set; } //Thông tin cơ bản của người dùng
  }

  // DTO cho thông tin người dùng (tùy chọn, để trả về sau khi login)
  public class UserDto
  {
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Role { get; set; }
  }

  public class AccessTokenString
  {
    public string accessToken { get; set; }
  }

  public class VerifyOTPRespone
  {
    public bool status { get; set; }
  }

  public class ResetPasswordRequest
  {
    public string newPassword { get; set; }
  }

  public class ForgetPasswordRequest
  {
    public string email{ get; set; }
    public string token{ get; set; }
    public string newPassword { get; set; }
  }
}