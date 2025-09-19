using LeOne.Application.Reviews.Dtos;

namespace LeOne.API.Controllers.Responses
{
    public sealed record CreateReviewResponse(string Message, ReviewDto ReviewDto);
}
