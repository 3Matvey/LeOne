using AutoMapper;
using FluentValidation;
using LeOne.Application.Common;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.SpaServices.Dtos;

namespace LeOne.Application.SpaServices.Commands.UpdateSpaService
{
    public sealed class ChangeSpaServicePriceHandler(IUnitOfWork uow, IValidator<ChangeSpaServicePriceCommand> validator, IDomainEventBus bus, IMapper mapper)
        : IChangeSpaServicePrice
    {
        public async Task<Result<SpaServiceDto>> HandleAsync(ChangeSpaServicePriceCommand cmd, CancellationToken ct = default)
        {
            var vr = await validator.ValidateAsync(cmd, ct);
            if (!vr.IsValid)
                return Error.BadRequest("SpaService.ChangePrice.BadRequest", vr.ToString());

            var entity = await uow.SpaService.FirstOrDefaultAsync(x => x.Id == cmd.Id, ct);
            if (entity is null)
                return Error.NotFound("SpaService.NotFound", $"SpaService {cmd.Id}");

            try
            {
                var priceChangedEvent = entity.ChangePrice(cmd.NewPriceInCents);

                await uow.ExecuteInTransactionAsync(async innerCt =>
                {
                    await uow.SpaService.UpdateAsync(entity, innerCt);
                }, ct).PublishIfOk(bus, ct, priceChangedEvent);

                var dto = mapper.Map<SpaServiceDto>(entity);

                return Result<SpaServiceDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Error.Failure("SpaService.ChangePrice.Failure", ex.Message);
            }
        }
    }
}
