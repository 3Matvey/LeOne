using LeOne.Application.Common.Results;

namespace LeOne.Application.SpaServices.Commands.DeleteSpaService
{
    public interface IDeleteSpaService
    {
        Task<Result> HandleAsync(DeleteSpaServiceCommand cmd, CancellationToken ct = default);
    }
}
