namespace LeOne.Application.Auth
{
    public sealed record AuthTokens(
        string AccessToken,
        DateTimeOffset AccessTokenExpiresAt,
        string RefreshToken,
        DateTimeOffset RefreshTokenExpiresAt,
        string TokenType = "Bearer"
    );
}
