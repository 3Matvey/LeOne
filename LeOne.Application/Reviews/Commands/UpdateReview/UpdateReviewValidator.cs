using FluentValidation;

namespace LeOne.Application.Reviews.Commands.UpdateReview
{
    public sealed class UpdateReviewValidator : AbstractValidator<UpdateReviewCommand>
    {
        public UpdateReviewValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Mark).InclusiveBetween((byte)1, (byte)5);
            RuleFor(x => x.Description).MaximumLength(2000);
        }
    }
}
