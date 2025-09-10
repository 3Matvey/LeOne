using LeOne.Application.Common.Results;

namespace LeOne.Application.Products.Commands.DeleteProduct
{
    public interface IDeleteProduct
    {
        Task<Result> HandleAsync(DeleteProductCommand cmd, CancellationToken ct = default);
    }
}
