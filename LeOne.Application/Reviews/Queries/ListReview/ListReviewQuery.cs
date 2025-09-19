namespace LeOne.Application.Reviews.Queries.ListReview
{
    public sealed record ListReviewQuery(
        int Page,
        int PageSize,
        Guid? EntityId);
}
