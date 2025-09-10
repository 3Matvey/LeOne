namespace LeOne.Application.Products.Dtos
{
    public sealed record ProductDto(
        Guid Id,
        string Name,
        long PriceInCents,
        DateTimeOffset? OrderedAt,
        string? Description,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt);
}
