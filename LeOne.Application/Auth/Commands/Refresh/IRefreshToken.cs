using LeOne.Application.Common.Results;

namespace LeOne.Application.Auth.Commands.Refresh
{
    public interface IRefreshToken
    {
        Task<Result<AuthTokens>> HandleAsync(RefreshTokenCommand cmd, CancellationToken ct = default);
    }
}