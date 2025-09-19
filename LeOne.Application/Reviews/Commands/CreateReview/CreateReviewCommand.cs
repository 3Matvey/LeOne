namespace LeOne.Application.Reviews.Commands.CreateReview
{
    public sealed record CreateReviewCommand(
        Guid EntityId,
        byte Mark,
        string? Description);
}
