using LeOne.Application.Common.Results;

namespace LeOne.Application.Reviews.Commands.DeleteReview
{
    public interface IDeleteReview
    {
        Task<Result> HandleAsync(DeleteReviewCommand cmd, CancellationToken ct = default);
    }
}
