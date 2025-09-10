using LeOne.Application.Common.Results;
using LeOne.Application.Products.Dtos;

namespace LeOne.Application.Products.Commands.UpdateProduct
{
    public interface IChangeProductPrice
    {
        Task<Result<ProductDto>> HandleAsync(ChangeProductPriceCommand cmd, CancellationToken ct = default);
    }
}
