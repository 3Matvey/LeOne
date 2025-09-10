namespace LeOne.Application.Products.Queries.ListProduct
{
    public record ListProductQuery(
        string? NameFilter = null,
        long? MinPriceInCents = null,
        long? MaxPriceInCents = null,
        int Page = 1,
        int PageSize = 10);
 }
