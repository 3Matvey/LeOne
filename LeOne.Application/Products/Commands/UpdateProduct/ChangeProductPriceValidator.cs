using FluentValidation;

namespace LeOne.Application.Products.Commands.UpdateProduct
{
    public sealed class ChangeProductPriceValidator : AbstractValidator<ChangeProductPriceCommand>
    {
        public ChangeProductPriceValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.NewPriceInCents).GreaterThanOrEqualTo(0);
        }
    }
}
