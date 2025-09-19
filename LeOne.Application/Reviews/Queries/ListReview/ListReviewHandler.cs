using AutoMapper;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.Reviews.Dtos;
using LeOne.Domain.Entities;
using System.Linq.Expressions;
using System.Linq;

namespace LeOne.Application.Reviews.Queries.ListReview
{
    public sealed class ListReviewHandler(IUnitOfWork uow, IMapper mapper)
    {
        public async Task<Result<ListReviewResponse>> HandleAsync(ListReviewQuery query, CancellationToken ct = default)
        {
            Expression<Func<Review, bool>>? filter = null;
            if (query.EntityId.HasValue)
                filter = x => x.EntityId == query.EntityId.Value;

            var skip = (query.Page - 1) * query.PageSize;

            var items = await uow.Reviews.ListAsync(filter, skip, query.PageSize, ct);
            var totalCount = await uow.Reviews.CountAsync(filter, ct);

            var ordered = items.OrderByDescending(x => x.CreatedAt).ToList();

            var dtos = mapper.Map<List<ReviewDto>>(ordered);

            return Result<ListReviewResponse>.Success(new ListReviewResponse(dtos, totalCount, query.Page, query.PageSize));
        }
    }
}
