namespace HomeTrack.Application.Interface
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(int userId);
        Task<bool> VerifyTokenAsync(int userId, string token);
        Task RevokeAllUserOTP(int userId);
    }
}