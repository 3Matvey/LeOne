namespace LeOne.Infrastructure.Auth
{
    public class JwtSettings
    {
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public string Key { get; init; } = string.Empty;
        public int ExpiryMinutes { get; init; }
        public int RefreshTokenExpiryDays { get; init; }
    }
}
