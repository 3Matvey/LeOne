using AutoMapper;
using FluentValidation;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.Reviews.Dtos;
using LeOne.Domain.Entities;

namespace LeOne.Application.Reviews.Commands.CreateReview
{
    public sealed class CreateReviewHandler(IUnitOfWork uow, IValidator<CreateReviewCommand> validator, IMapper mapper)
        : ICreateReview
    {
        public async Task<Result<ReviewDto>> HandleAsync(CreateReviewCommand cmd, CancellationToken ct = default)
        {
            var vr = await validator.ValidateAsync(cmd, ct);
            if (!vr.IsValid)
                return Error.BadRequest("Review.Create.BadRequest", vr.ToString());

            var review = new Review(cmd.EntityId, cmd.Mark, cmd.Description);

            await uow.ExecuteInTransactionAsync(async innerCt =>
            {
                await uow.Reviews.AddAsync(review, innerCt);
            }, ct);

            var dto = mapper.Map<ReviewDto>(review);
            return Result<ReviewDto>.Success(dto);
        }
    }
}
