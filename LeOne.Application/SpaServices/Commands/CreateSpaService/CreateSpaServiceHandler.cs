using AutoMapper;
using FluentValidation;
using LeOne.Application.Common;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.SpaServices.Dtos;
using LeOne.Domain.Entities;

namespace LeOne.Application.SpaServices.Commands.CreateSpaService
{
    public sealed class CreateSpaServiceHandler(IUnitOfWork uow, IValidator<CreateSpaServiceCommand> validator, IDomainEventBus bus, IMapper mapper)
        : ICreateSpaService
    {
        public async Task<Result<SpaServiceDto>> HandleAsync(CreateSpaServiceCommand cmd, CancellationToken ct = default)
        {
            var vr = await validator.ValidateAsync(cmd, ct);
            if (!vr.IsValid)
                return Error.BadRequest("SpaService.Create.BadRequest", vr.ToString());

            var (spaService, createdEvent) = SpaService.Create(cmd.Name, cmd.PriceInCents, cmd.DurationMinutes, cmd.Description);
                
            await uow.ExecuteInTransactionAsync(async innerCt =>
            {
                await uow.SpaService.AddAsync(spaService, innerCt);
            }, ct).PublishIfOk(bus, ct, createdEvent);

            var dto = mapper.Map<SpaServiceDto>(spaService);

            return Result<SpaServiceDto>.Success(dto);
        }
    }
}