namespace LeOne.Application.Reviews.Commands.UpdateReview
{
    public sealed record UpdateReviewCommand(
        Guid Id,
        byte Mark,
        string? Description);
}
