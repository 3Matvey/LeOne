using FluentValidation;

namespace LeOne.Application.Reviews.Commands.CreateReview
{
    public sealed class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.EntityId).NotEmpty();
            RuleFor(x => x.Mark).InclusiveBetween((byte)1, (byte)5);
            RuleFor(x => x.Description).MaximumLength(2000);
        }
    }
}
