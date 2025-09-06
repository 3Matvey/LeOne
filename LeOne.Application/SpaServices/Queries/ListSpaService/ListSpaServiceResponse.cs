using LeOne.Application.SpaServices.Dtos;

namespace LeOne.Application.SpaServices.Queries.ListSpaService;

public record ListSpaServiceResponse(
    IReadOnlyList<SpaServiceDto> Items,
    int TotalCount,
    int Page,
    int PageSize);