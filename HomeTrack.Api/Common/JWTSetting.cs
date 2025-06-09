namespace HomeTrack.Infrastructure.Jwt
{
    public class JwtSetting
    {
        public string SecretKey { get; set; } = string.Empty;
        public int AccessExpiresInMinutes { get; set; }
        public int RefreshExpiresInMinutes { get; set; }
    }
}
