using LeOne.Application.Common.Results;
using LeOne.Application.SpaServices.Dtos;

namespace LeOne.Application.SpaServices.Commands.CreateSpaService
{
    public interface ICreateSpaService
    {
        Task<Result<SpaServiceDto>> HandleAsync(CreateSpaServiceCommand cmd, CancellationToken ct = default);
    }
}
