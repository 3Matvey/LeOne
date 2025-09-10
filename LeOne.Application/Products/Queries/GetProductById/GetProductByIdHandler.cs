using AutoMapper;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.Products.Dtos;

namespace LeOne.Application.Products.Queries.GetProductById
{
    public class GetProductByIdHandler(
        IUnitOfWork uow,
        IMapper mapper)
    {
        public async Task<Result<ProductDto>> HandleAsync(GetProductByIdQuery query, CancellationToken ct = default)
        {
            var entity = await uow.Products.FirstOrDefaultAsync(x => x.Id == query.Id, ct);
            if (entity is null)
                return Error.NotFound("Product.NotFound", $"Product {query.Id}");

            var dto = mapper.Map<ProductDto>(entity);

            return Result<ProductDto>.Success(dto);
        }
    }
}
