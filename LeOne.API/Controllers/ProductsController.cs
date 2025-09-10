using LeOne.Application.Products.Commands.CreateProduct;
using LeOne.Application.Products.Commands.DeleteProduct;
using LeOne.Application.Products.Commands.UpdateProduct;
using LeOne.Application.Products.Queries.GetProductById;
using LeOne.Application.Products.Queries.ListProduct;
using LeOne.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace LeOne.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(
        ICreateProduct create,
        IChangeProductPrice changePrice,
        IDeleteProduct delete,
        GetProductByIdHandler getById,
        ListProductHandler list) : ControllerBaseWithResult
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ListProductQuery query, CancellationToken ct)
        {
            var res = await list.HandleAsync(query, ct);
            return res.Match(Ok, Problem);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var res = await getById.HandleAsync(new GetProductByIdQuery(id), ct);
            return res.Match(Ok, Problem);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand cmd, CancellationToken ct)
        {
            var res = await create.HandleAsync(cmd, ct);

            return res.Match(
                onSuccess: r =>
                {
                    var body = new
                    {
                        Message = "Product created successfully",
                        SpaServiceDto = r
                    };

                    return CreatedAtRoute(
                        routeValues: new { r.Id },
                        value: body);
                },
                onFailure: Problem
            );
        }

        [HttpPatch("{id:guid}/price")]
        public async Task<IActionResult> ChangePrice([FromRoute] Guid id, [FromBody] ChangePriceBody body, CancellationToken ct)
        {
            var res = await changePrice.HandleAsync(new ChangeProductPriceCommand(id, body.NewPriceInCents), ct);

            return res.Match(Ok, Problem);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var res = await delete.HandleAsync(new DeleteProductCommand(id), ct);

            return res.Match(Ok, Problem);
        }

        public sealed record ChangePriceBody(long NewPriceInCents);
    }
}
