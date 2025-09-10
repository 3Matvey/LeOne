namespace LeOne.Application.Products.Commands.CreateProduct
{
    public sealed record CreateProductCommand(
        string Name,
        long PriceInCents,
        string? Description);
}