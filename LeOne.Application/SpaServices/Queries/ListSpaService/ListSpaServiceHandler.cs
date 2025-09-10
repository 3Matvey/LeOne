using AutoMapper;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.SpaServices.Dtos;
using LeOne.Domain.Entities;
using System.Linq.Expressions;

namespace LeOne.Application.SpaServices.Queries.ListSpaService;

public class ListSpaServiceHandler(IUnitOfWork unitOfWork, IMapper mapper)
{

    public async Task<Result<ListSpaServiceResponse>> HandleAsync(
        ListSpaServiceQuery request,
        CancellationToken ct)
    {
        Expression<Func<SpaService, bool>>? filter = BuildFilter(request);

        var skip = (request.Page - 1) * request.PageSize;
        
        var items = await unitOfWork.SpaService.ListAsync(
            filter,
            skip,
            request.PageSize,
            ct);

        var totalCount = await unitOfWork.SpaService.CountAsync(filter, ct);

        var dtos = mapper.Map<List<SpaServiceDto>>(items);

        return Result<ListSpaServiceResponse>.Success(new ListSpaServiceResponse(
            dtos,
            totalCount,
            request.Page,
            request.PageSize));
    }

    private static Expression<Func<SpaService, bool>>? BuildFilter(ListSpaServiceQuery request)
    {
        ParameterExpression param = Expression.Parameter(typeof(SpaService), "s");
        Expression? body = null;

        // Name
        if (!string.IsNullOrWhiteSpace(request.NameFilter))
        {
            var prop = Expression.Property(param, nameof(SpaService.Name));
            var method = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
            var call = Expression.Call(prop, method, Expression.Constant(request.NameFilter));
            body = body == null ? call : Expression.AndAlso(body, call);
        }

        // MinPrice
        if (request.MinPriceInCents.HasValue)
        {
            var prop = Expression.Property(param, nameof(SpaService.PriceInCents)); 
            var value = Expression.Constant(request.MinPriceInCents.Value, typeof(long));
            var comparison = Expression.GreaterThanOrEqual(prop, value);
            body = body == null ? comparison : Expression.AndAlso(body, comparison);
        }

        // MaxPrice
        if (request.MaxPriceInCents.HasValue)
        {
            var prop = Expression.Property(param, nameof(SpaService.PriceInCents)); 
            var value = Expression.Constant(request.MaxPriceInCents.Value, typeof(long));
            var comparison = Expression.LessThanOrEqual(prop, value);
            body = body == null ? comparison : Expression.AndAlso(body, comparison);
        }

        // MinDuration
        if (request.MinDuration.HasValue)
        {
            var prop = Expression.Property(param, nameof(SpaService.DurationMinutes));
            var value = Expression.Constant(request.MinDuration.Value, typeof(int));
            var comparison = Expression.GreaterThanOrEqual(prop, value);
            body = body == null ? comparison : Expression.AndAlso(body, comparison);
        }

        // MaxDuration
        if (request.MaxDuration.HasValue)
        {
            var prop = Expression.Property(param, nameof(SpaService.DurationMinutes));
            var value = Expression.Constant(request.MaxDuration.Value, typeof(int));
            var comparison = Expression.LessThanOrEqual(prop, value);
            body = body == null ? comparison : Expression.AndAlso(body, comparison);
        }

        return body is null ? null : Expression.Lambda<Func<SpaService, bool>>(body, param);
    }
}
