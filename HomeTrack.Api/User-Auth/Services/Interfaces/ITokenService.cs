namespace HomeTrack.Application.Interface
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(int userId);
        Task<int?> VerifyTokenAsync(string token);
    }
}