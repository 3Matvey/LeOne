using LeOne.Application.Common.Results;
using LeOne.Application.Reviews.Dtos;

namespace LeOne.Application.Reviews.Commands.CreateReview
{
    public interface ICreateReview
    {
        Task<Result<ReviewDto>> HandleAsync(CreateReviewCommand cmd, CancellationToken ct = default);
    }
}
