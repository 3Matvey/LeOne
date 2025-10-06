namespace LeOne.Application.Reviews.Commands.CreateReview
{
    public sealed record CreateReviewCommand(
        Guid EntityId,
        Guid CreatedByUserId,
        byte Mark,
        string? Description);
}
