using AutoMapper;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.Reviews.Dtos;

namespace LeOne.Application.Reviews.Queries.GetReviewById
{
    public sealed class GetReviewByIdHandler(IUnitOfWork uow, IMapper mapper)
    {
        public async Task<Result<ReviewDto>> HandleAsync(GetReviewByIdQuery query, CancellationToken ct = default)
        {
            var entity = await uow.Reviews.FirstOrDefaultAsync(x => x.Id == query.Id, ct);
            if (entity is null)
                return Error.NotFound("Review.NotFound", $"Review {query.Id}");

            var dto = mapper.Map<ReviewDto>(entity);
            return Result<ReviewDto>.Success(dto);
        }
    }
}
