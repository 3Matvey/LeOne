using LeOne.Application.Common.Results;
using LeOne.Application.Reviews.Commands.CreateReview;
using LeOne.Application.Reviews.Commands.DeleteReview;
using LeOne.Application.Reviews.Commands.UpdateReview;
using LeOne.Application.Reviews.Queries.GetReviewById;
using LeOne.Application.Reviews.Queries.ListReview;
using Microsoft.AspNetCore.Mvc;

namespace LeOne.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class ReviewsController(
        ICreateReview create,
        IUpdateReview update,
        IDeleteReview delete,
        GetReviewByIdHandler getById,
        ListReviewHandler list)
        : ControllerBaseWithResult
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] ListReviewQuery query, CancellationToken ct)
        {
            var res = await list.HandleAsync(query, ct);
            return res.Match(Ok, Problem);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
        {
            var res = await getById.HandleAsync(new GetReviewByIdQuery(id), ct);
            return res.Match(Ok, Problem);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewCommand cmd, CancellationToken ct)
        {
            var res = await create.HandleAsync(cmd, ct);

            return res.Match(
                onSuccess: r =>
                {
                    var body = new LeOne.API.Controllers.Responses.CreateReviewResponse(
                        "Review created successfully",
                        r);

                    return CreatedAtRoute(routeValues: new { r.Id }, value: body);
                },
                onFailure: Problem
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateReviewCommand cmd, CancellationToken ct)
        {
            var res = await update.HandleAsync(cmd, ct);
            return res.Match(Ok, Problem);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
        {
            var res = await delete.HandleAsync(new DeleteReviewCommand(id), ct);
            return res.Match(Ok, Problem);
        }
    }
}
