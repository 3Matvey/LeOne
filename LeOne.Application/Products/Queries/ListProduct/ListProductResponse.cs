using LeOne.Application.Products.Dtos;

namespace LeOne.Application.Products.Queries.ListProduct
{
    public record ListProductResponse(
        IReadOnlyList<ProductDto> Items,
        int TotalCount,
        int Page,
        int PageSize);
}
