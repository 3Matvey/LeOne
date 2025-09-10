namespace LeOne.Application.SpaServices.Queries.ListSpaService;

public record ListSpaServiceQuery(
    string? NameFilter = null,
    long? MinPriceInCents = null,
    long? MaxPriceInCents = null,
    int? MinDuration = null,
    int? MaxDuration = null,
    int Page = 1,
    int PageSize = 10);