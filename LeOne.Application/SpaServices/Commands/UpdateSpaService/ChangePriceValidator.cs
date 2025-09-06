using FluentValidation;

namespace LeOne.Application.SpaServices.Commands.UpdateSpaService
{
    public sealed class ChangePriceValidator : AbstractValidator<ChangePriceCommand>
    {
        public ChangePriceValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.NewPriceInCents).GreaterThanOrEqualTo(0);
        }
    }
}
