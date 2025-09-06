namespace LeOne.Application.SpaServices.Dtos;

public sealed record SpaServiceDto(
    Guid Id,
    string Name,
    long PriceInCents,
    int DurationMinutes,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
