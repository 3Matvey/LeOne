using System.Net;
using System.Net.Http.Json;
using LeOne.Application.Reviews.Commands.CreateReview;
using LeOne.Application.Reviews.Commands.UpdateReview;
using LeOne.Application.Reviews.Dtos;
using LeOne.Application.Reviews.Queries.ListReview;

namespace LeOne.API.E2ETests;

[Collection("E2E Database")]
public class ReviewsControllerTests
{
    private record CreateReviewResponse(string Message, ReviewDto ReviewDto);

    [Fact]
    public async Task Create_ReturnsCreatedReview()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var cmd = new CreateReviewCommand(Guid.NewGuid(), 5, "Great");
        var response = await client.PostAsJsonAsync("/api/Reviews", cmd);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateReviewResponse>();
        Assert.NotNull(result);
        Assert.Equal(cmd.Mark, result!.ReviewDto.Mark);
        Assert.Equal(cmd.Description, result.ReviewDto.Description);
    }

    [Fact]
    public async Task GetById_ReturnsReview()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var entityId = Guid.NewGuid();
        var cmd = new CreateReviewCommand(entityId, 4, "Nice");
        var createResp = await client.PostAsJsonAsync("/api/Reviews", cmd);
        var created = await createResp.Content.ReadFromJsonAsync<CreateReviewResponse>();
        var id = created!.ReviewDto.Id;

        var getResp = await client.GetAsync($"/api/Reviews/{id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
        var dto = await getResp.Content.ReadFromJsonAsync<ReviewDto>();
        Assert.NotNull(dto);
        Assert.Equal(cmd.Mark, dto!.Mark);
    }

    [Fact]
    public async Task List_ReturnsPaginatedResults()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // baseline
        var baselineResp = await client.GetAsync("/api/Reviews?page=1&pageSize=1");
        Assert.Equal(HttpStatusCode.OK, baselineResp.StatusCode);
        var baselineBody = await baselineResp.Content.ReadFromJsonAsync<ListReviewResponse>();
        Assert.NotNull(baselineBody);
        var baselineCount = baselineBody!.TotalCount;

        var entityId = Guid.NewGuid();
        await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(entityId, 1, null));
        await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(entityId, 2, null));
        await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(entityId, 3, null));

        var expectedTotal = baselineCount + 3;

        var lastPage = expectedTotal;
        var listResp = await client.GetAsync($"/api/Reviews?page={lastPage}&pageSize=1");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);

        var body = await listResp.Content.ReadFromJsonAsync<ListReviewResponse>();
        Assert.NotNull(body);
        Assert.Equal(expectedTotal, body!.TotalCount);
        Assert.Equal(lastPage, body.Page);

        var item = Assert.Single(body.Items);
        Assert.Equal(3, item.Mark);
    }

    [Fact]
    public async Task Update_UpdatesReview()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var entityId = Guid.NewGuid();
        var createResp = await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(entityId, 2, "ok"));
        var created = await createResp.Content.ReadFromJsonAsync<CreateReviewResponse>();
        var id = created!.ReviewDto.Id;

        var updateCmd = new UpdateReviewCommand(id, 5, "excellent");
        var putResp = await client.PutAsJsonAsync($"/api/Reviews/{id}", updateCmd);
        Assert.Equal(HttpStatusCode.OK, putResp.StatusCode);

        var updated = await putResp.Content.ReadFromJsonAsync<ReviewDto>();
        Assert.NotNull(updated);
        Assert.Equal(5, updated!.Mark);
    }

    [Fact]
    public async Task Delete_RemovesReview()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var createResp = await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(Guid.NewGuid(), 3, null));
        var created = await createResp.Content.ReadFromJsonAsync<CreateReviewResponse>();
        var id = created!.ReviewDto.Id;

        var delResp = await client.DeleteAsync($"/api/Reviews/{id}");
        Assert.Equal(HttpStatusCode.OK, delResp.StatusCode);

        var getResp = await client.GetAsync($"/api/Reviews/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }
}
