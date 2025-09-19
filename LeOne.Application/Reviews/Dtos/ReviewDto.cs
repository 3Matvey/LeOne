namespace LeOne.Application.Reviews.Dtos
{
    public sealed record ReviewDto(
        Guid Id,
        Guid EntityId,
        byte Mark,
        string? Description,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt);
}
