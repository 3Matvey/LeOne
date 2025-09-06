using LeOne.Application.Common.Results;
using LeOne.Application.SpaServices.Commands.CreateSpaService;
using LeOne.Application.SpaServices.Commands.DeleteSpaService;
using LeOne.Application.SpaServices.Commands.UpdateSpaService;
using LeOne.Application.SpaServices.Queries.GetSpaServiceById;
using LeOne.Application.SpaServices.Queries.ListSpaService;
using Microsoft.AspNetCore.Mvc;

namespace LeOne.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class SpaServicesController(
        ICreateSpaService create,
        IChangePrice changePrice,
        IDeleteSpaService delete,
        GetSpaServiceByIdHandler getById,
        ListSpaServiceHandler list)
        : ControllerBaseWithResult
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ListSpaServiceQuery query, CancellationToken ct)
        {
            var res = await list.HandleAsync(query, ct);
            return res.Match(Ok, Problem);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var res = await getById.HandleAsync(new GetSpaServiceByIdQuery(id), ct);
            return res.Match(Ok, Problem);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSpaServiceCommand cmd, CancellationToken ct)
        {
            var res = await create.HandleAsync(cmd, ct);

            return res.Match(
                onSuccess: r =>
                {
                    var body = new
                    {
                        Message = "Voting session created successfully",
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
            var res = await changePrice.HandleAsync(new ChangePriceCommand(id, body.NewPriceInCents), ct);

            return res.Match(Ok, Problem);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var res = await delete.HandleAsync(new DeleteSpaServiceCommand(id), ct);

            return res.Match(Ok, Problem);
        }

        public sealed record ChangePriceBody(long NewPriceInCents);
    }
}
