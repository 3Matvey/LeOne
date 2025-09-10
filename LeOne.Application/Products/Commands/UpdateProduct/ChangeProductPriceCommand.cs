namespace LeOne.Application.Products.Commands.UpdateProduct
{
    public sealed record ChangeProductPriceCommand(Guid Id, long NewPriceInCents);
}
