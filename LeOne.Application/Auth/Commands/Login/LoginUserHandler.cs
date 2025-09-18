using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;

namespace LeOne.Application.Auth.Commands.Login
{
    public sealed class LoginUserHandler(
        IUnitOfWork uow,
        IPasswordHasher hasher,
        IJwtTokenGenerator tokenGenerator
    ) : ILoginUser
    {
        public async Task<Result<AuthTokens>> HandleAsync(LoginUserCommand cmd, CancellationToken ct = default)
        {
            var user = await uow.Users.FirstOrDefaultAsync(u => u.Email.Value == cmd.Email, ct);
            if (user is null)
                return Error.AccessUnAuthorized("Auth.InvalidCredentials", "Invalid email or password");

            var hashed = hasher.Hash(cmd.Password, user.Password.Salt);
            if (hashed != user.Password.Hash)
                return Error.AccessUnAuthorized("Auth.InvalidCredentials", "Invalid email or password");

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
