using AutoMapper;
using FluentValidation;
using LeOne.Application.Common;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.Products.Dtos;
using LeOne.Domain.Entities;

namespace LeOne.Application.Products.Commands.CreateProduct
{
    public sealed class CreateProductHandler(
        IUnitOfWork uow,
        IValidator<CreateProductCommand> validator,
        IDomainEventBus bus,
        IMapper mapper)
        : ICreateProduct
    {
        public async Task<Result<ProductDto>> HandleAsync(CreateProductCommand cmd, CancellationToken ct = default)
        {
            var vr = await validator.ValidateAsync(cmd, ct);
            if (!vr.IsValid)
                return Error.BadRequest("Product.Create.BadRequest", vr.ToString());

            var (product, createdEvent) = Product.Create(cmd.Name, cmd.PriceInCents, cmd.Description);

            await uow.ExecuteInTransactionAsync(async innerCt => 
            {
                await uow.Products.AddAsync(product, innerCt);
            }, ct).PublishIfOk(bus, ct, createdEvent);

            var dto = mapper.Map<ProductDto>(product);

            return Result<ProductDto>.Success(dto);
        }
    }
}
