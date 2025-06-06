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
    public required bool IsSuccess { get; set; }
    public string? AccessToken { get; set; } //Token để truy cập API
    public string? RefreshToken { get; set; } //Token để làm mới AccessToken
    public string? ErrorMessage { get; set; } //Thông báo lỗi nếu đăng nhập thất bại
    public UserDto? User { get; set; } //Thông tin cơ bản của người dùng
  }

  // DTO cho thông tin người dùng (tùy chọn, để trả về sau khi login)
  public class UserDto
  {
    public required int Id { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public required string Role { get; set; }
  }

  public class AccessTokenString
  {
    public required string accessToken { get; set; }
  }

  public class VerifyOTPRespone
  {
    public required bool status { get; set; }
  }

  public class ResetPasswordRequest
  {
    public required string newPassword { get; set; }
  }

  public class ForgetPasswordRequest
  {
    public required string email { get; set; }
    public required string token { get; set; }
    public required string newPassword { get; set; }
    public required string repeatPassword{ get; set; }
  }
}