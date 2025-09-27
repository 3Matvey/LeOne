using AutoMapper;
using FluentValidation;
using LeOne.Application.Common;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.Products.Dtos;

namespace LeOne.Application.Products.Commands.UpdateProduct
{
    public sealed class ChangeProductPriceHandler(IUnitOfWork uow, IValidator<ChangeProductPriceCommand> validator, IDomainEventBus bus, IMapper mapper)
        : IChangeProductPrice
    {
        public async Task<Result<ProductDto>> HandleAsync(ChangeProductPriceCommand cmd, CancellationToken ct = default)
        {
            var vr = await validator.ValidateAsync(cmd, ct);
            if (!vr.IsValid)
                return Error.BadRequest("Product.ChangePrice.BadRequest", vr.ToString());

            var entity = await uow.Products.FirstOrDefaultAsync(x => x.Id == cmd.Id, ct);
            if (entity is null)
                return Error.NotFound("Product.NotFound", $"Product {cmd.Id}");

            try
            {
                var @event = entity.ChangePrice(cmd.NewPriceInCents);

                await uow.ExecuteInTransactionAsync(async innerCt =>
                {
                    await uow.Products.UpdateAsync(entity, innerCt);
                }, ct).PublishIfOk(bus, @event, ct);

                var dto = mapper.Map<ProductDto>(entity);

                return Result<ProductDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Error.Failure("Product.ChangePrice.Failure", ex.Message);
            }
        }
    }
}
