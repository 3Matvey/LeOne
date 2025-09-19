using LeOne.Application.Common.Results;
using LeOne.Application.Reviews.Dtos;

namespace LeOne.Application.Reviews.Commands.UpdateReview
{
    public interface IUpdateReview
    {
        Task<Result<ReviewDto>> HandleAsync(UpdateReviewCommand cmd, CancellationToken ct = default);
    }
}
