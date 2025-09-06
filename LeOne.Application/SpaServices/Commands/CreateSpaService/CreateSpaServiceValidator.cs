using FluentValidation;

namespace LeOne.Application.SpaServices.Commands.CreateSpaService;

public sealed class CreateSpaServiceValidator : AbstractValidator<CreateSpaServiceCommand>
{
    public CreateSpaServiceValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PriceInCents).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DurationMinutes).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(2000);
    }
}
