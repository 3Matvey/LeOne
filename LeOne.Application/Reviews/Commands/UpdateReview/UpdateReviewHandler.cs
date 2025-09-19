using FluentValidation;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.Reviews.Dtos;
using AutoMapper;

namespace LeOne.Application.Reviews.Commands.UpdateReview
{
    public sealed class UpdateReviewHandler(IUnitOfWork uow, IValidator<UpdateReviewCommand> validator, IMapper mapper)
        : IUpdateReview
    {
        public async Task<Result<ReviewDto>> HandleAsync(UpdateReviewCommand cmd, CancellationToken ct = default)
        {
            var vr = await validator.ValidateAsync(cmd, ct);
            if (!vr.IsValid)
                return Error.BadRequest("Review.Update.BadRequest", vr.ToString());

            var entity = await uow.Reviews.FirstOrDefaultAsync(x => x.Id == cmd.Id, ct);
            if (entity is null)
                return Error.NotFound("Review.NotFound", $"Review {cmd.Id}");

            entity.Edit(cmd.Mark, cmd.Description);

            await uow.ExecuteInTransactionAsync(async innerCt =>
            {
                await uow.Reviews.UpdateAsync(entity, innerCt);
            }, ct);

            var dto = mapper.Map<ReviewDto>(entity);
            return Result<ReviewDto>.Success(dto);
        }
    }
}
