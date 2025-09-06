using LeOne.Application.Common.Results;
using LeOne.Application.SpaServices.Dtos;

namespace LeOne.Application.SpaServices.Commands.UpdateSpaService
{
    public interface IChangePrice
    {
        Task<Result<SpaServiceDto>> HandleAsync(ChangePriceCommand cmd, CancellationToken ct = default);
    }
}
