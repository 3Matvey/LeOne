using LeOne.Application.Common.Results;
using LeOne.Application.Products.Dtos;

namespace LeOne.Application.Products.Commands.CreateProduct
{
    public interface ICreateProduct
    {
        Task<Result<ProductDto>> HandleAsync(CreateProductCommand cmd, CancellationToken ct = default);
    }
}
