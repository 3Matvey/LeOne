using LeOne.Application.Common.Results;
using LeOne.Application.SpaServices.Dtos;

namespace LeOne.Application.SpaServices.Commands.UpdateSpaService
{
    public interface IChangeSpaServicePrice
    {
        Task<Result<SpaServiceDto>> HandleAsync(ChangeSpaServicePriceCommand cmd, CancellationToken ct = default);
    }
}
