using LeOne.Application.Reviews.Dtos;

namespace LeOne.Application.Reviews.Queries.ListReview
{
    public record ListReviewResponse(
        IReadOnlyList<ReviewDto> Items,
        int TotalCount,
        int Page,
        int PageSize);
}
