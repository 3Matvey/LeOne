using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;

namespace LeOne.Application.Products.Commands.DeleteProduct
{
    public sealed class DeleteProductHandler(IUnitOfWork uow) : IDeleteProduct
    {
        public async Task<Result> HandleAsync(DeleteProductCommand cmd, CancellationToken ct = default)
        {
            var entity = await uow.Products.FirstOrDefaultAsync(x => x.Id == cmd.Id, ct);
            if (entity is null)
                return Error.NotFound("Product.NotFound", $"Product {cmd.Id}");

            try
            {
                await uow.ExecuteInTransactionAsync(async innerCt =>
                {
                    await uow.Products.DeleteAsync(entity, innerCt);
                }, ct);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Error.Failure("Product.Delete.Failure", ex.Message);
            }
        }
    }
}
