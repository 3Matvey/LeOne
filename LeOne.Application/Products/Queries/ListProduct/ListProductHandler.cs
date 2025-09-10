using AutoMapper;
using LeOne.Application.Common.Interfaces;
using LeOne.Application.Common.Results;
using LeOne.Application.Products.Dtos;
using LeOne.Domain.Entities;
using System.Linq.Expressions;

namespace LeOne.Application.Products.Queries.ListProduct;

public class ListProductHandler(IUnitOfWork unitOfWork, IMapper mapper)
{
    public async Task<Result<ListProductResponse>> HandleAsync(
        ListProductQuery request,
        CancellationToken ct)
    {
        Expression<Func<Product, bool>>? filter = BuildFilter(request);

        var skip = (request.Page - 1) * request.PageSize;

        var items = await unitOfWork.Products.ListAsync(
            filter,
            skip,
            request.PageSize,
            ct);

        var totalCount = await unitOfWork.Products.CountAsync(filter, ct);

        var dtos = mapper.Map<List<ProductDto>>(items);

        return Result<ListProductResponse>.Success(new ListProductResponse(
            dtos,
            totalCount,
            request.Page,
            request.PageSize));
    }

    private static Expression<Func<Product, bool>>? BuildFilter(ListProductQuery request)
    {
        ParameterExpression param = Expression.Parameter(typeof(Product), "s");
        Expression? body = null;

        // Name
        if (!string.IsNullOrWhiteSpace(request.NameFilter))
        {
            var prop = Expression.Property(param, nameof(Product.Name));
            var method = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
            var call = Expression.Call(prop, method, Expression.Constant(request.NameFilter));
            body = body is null ? call : Expression.AndAlso(body, call);
        }

        // MinPrice
        if (request.MinPriceInCents.HasValue)
        {
            var prop = Expression.Property(param, nameof(Product.PriceInCents)); 
            var value = Expression.Constant(request.MinPriceInCents.Value, typeof(long)); 
            var comparison = Expression.GreaterThanOrEqual(prop, value);
            body = body is null ? comparison : Expression.AndAlso(body, comparison);
        }

        // MaxPrice
        if (request.MaxPriceInCents.HasValue)
        {
            var prop = Expression.Property(param, nameof(Product.PriceInCents)); 
            var value = Expression.Constant(request.MaxPriceInCents.Value, typeof(long)); 
            var comparison = Expression.LessThanOrEqual(prop, value);
            body = body is null ? comparison : Expression.AndAlso(body, comparison);
        }

        return body is null ? null : Expression.Lambda<Func<Product, bool>>(body, param);
    }
}
