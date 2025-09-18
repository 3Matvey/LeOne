using LeOne.Application.Common.Results;

namespace LeOne.Application.Auth.Commands.Login
{
    public interface ILoginUser
    {
        Task<Result<AuthTokens>> HandleAsync(LoginUserCommand cmd, CancellationToken ct = default);
    }
}