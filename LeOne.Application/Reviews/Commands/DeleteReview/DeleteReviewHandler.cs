using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;

namespace LeOne.Application.Reviews.Commands.DeleteReview
{
    public sealed class DeleteReviewHandler(IUnitOfWork uow)
        : IDeleteReview
    {
        public async Task<Result> HandleAsync(DeleteReviewCommand cmd, CancellationToken ct = default)
        {
            var entity = await uow.Reviews.FirstOrDefaultAsync(x => x.Id == cmd.Id, ct);
            if (entity is null)
                return Error.NotFound("Review.NotFound", $"Review {cmd.Id}");

            await uow.ExecuteInTransactionAsync(async innerCt =>
            {
                await uow.Reviews.DeleteAsync(entity, innerCt);
            }, ct);

            return Result.Success();
        }
    }
}
