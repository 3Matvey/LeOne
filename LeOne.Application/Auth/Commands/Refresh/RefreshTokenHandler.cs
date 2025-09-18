using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;

namespace LeOne.Application.Auth.Commands.Refresh
{
    public sealed class RefreshTokenHandler(
        IUnitOfWork uow,
        IJwtTokenGenerator tokenGenerator
    ) : IRefreshToken
    {
        public async Task<Result<AuthTokens>> HandleAsync(RefreshTokenCommand cmd, CancellationToken ct = default)
        {
            var (userId, jti) = tokenGenerator.ValidateRefreshTokenRich(cmd.Token);
            if (userId is null || jti is null)
                return Error.AccessUnAuthorized("Auth.InvalidRefresh", "Invalid refresh token");

            var user = await uow.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user is null)
                return Error.AccessUnAuthorized("Auth.InvalidRefresh", "Invalid refresh token");

            var (accessToken, accessExp) = tokenGenerator.GenerateAccessToken(user);
            var (refreshToken, refreshExp, _) = tokenGenerator.GenerateRefreshToken(user);

            var payload = new AuthTokens(
                AccessToken: accessToken,
                AccessTokenExpiresAt: accessExp,
                RefreshToken: refreshToken,
                RefreshTokenExpiresAt: refreshExp
            );

            return Result<AuthTokens>.Success(payload);
        }
    }
}
