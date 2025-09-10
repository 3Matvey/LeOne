using FluentValidation;

namespace LeOne.Application.SpaServices.Commands.UpdateSpaService
{
    public sealed class ChangeSpaServicePriceValidator : AbstractValidator<ChangeSpaServicePriceCommand>
    {
        public ChangeSpaServicePriceValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.NewPriceInCents).GreaterThanOrEqualTo(0);
        }
    }
}
