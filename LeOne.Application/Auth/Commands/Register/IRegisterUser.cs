using LeOne.Application.Common.Results;

namespace LeOne.Application.Auth.Commands.Register
{
    public interface IRegisterUser
    {
        Task<Result<AuthTokens>> HandleAsync(RegisterUserCommand cmd, CancellationToken ct = default);
    }
}