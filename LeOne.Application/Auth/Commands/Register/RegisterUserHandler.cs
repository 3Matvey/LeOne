using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Domain.Entities;
using LeOne.Domain.Shared;
using LeOne.Domain.ValueObjects;

namespace LeOne.Application.Auth.Commands.Register
{
    public sealed class RegisterUserHandler(
        IUnitOfWork uow,
        IPasswordHasher hasher,
        IJwtTokenGenerator tokenGenerator
    ) : IRegisterUser
    {
        public async Task<Result<AuthTokens>> HandleAsync(RegisterUserCommand cmd, CancellationToken ct = default)
        {
            var existing = await uow.Users.FirstOrDefaultAsync(u => u.Email.Value == cmd.Email, ct);
            if (existing is not null)
                return Error.Conflict("Auth.UserExists", "User already exists");

            var salt = Guid.NewGuid().ToString();
            var hash = hasher.Hash(cmd.Password, salt);

            var user = new User(
                cmd.FirstName,
                cmd.LastName,
                Email.Create(cmd.Email),
                PasswordHash.Create(hash, salt),
                UserRole.User
            );

            var (accessToken, accessExp) = tokenGenerator.GenerateAccessToken(user);
            var (refreshToken, refreshExp, _) = tokenGenerator.GenerateRefreshToken(user);

            await uow.Users.AddAsync(user, ct);
            await uow.SaveChangesAsync(ct);

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
