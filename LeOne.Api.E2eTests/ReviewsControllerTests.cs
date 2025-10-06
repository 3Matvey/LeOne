using System.Net;
using System.Net.Http.Json;
using LeOne.Application.Reviews.Commands.CreateReview;
using LeOne.Application.Reviews.Commands.UpdateReview;
using LeOne.Application.Reviews.Dtos;
using LeOne.Application.Reviews.Queries.ListReview;
using LeOne.Domain.Entities;
using LeOne.Domain.Shared;
using LeOne.Domain.ValueObjects;
using LeOne.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

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

        var userId = await CreateUserAsync(factory);
        var cmd = new CreateReviewCommand(Guid.NewGuid(), userId, 5, "Great");
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
        var userId = await CreateUserAsync(factory);
        var cmd = new CreateReviewCommand(entityId, userId, 4, "Nice");
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
        var userId = await CreateUserAsync(factory);
        await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(entityId, userId, 1, null));
        await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(entityId, userId, 2, null));
        await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(entityId, userId, 3, null));

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
        var userId = await CreateUserAsync(factory);
        var createResp = await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(entityId, userId, 2, "ok"));
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

        var userId = await CreateUserAsync(factory);
        var createResp = await client.PostAsJsonAsync("/api/Reviews", new CreateReviewCommand(Guid.NewGuid(), userId, 3, null));
        var created = await createResp.Content.ReadFromJsonAsync<CreateReviewResponse>();
        var id = created!.ReviewDto.Id;

        var delResp = await client.DeleteAsync($"/api/Reviews/{id}");
        Assert.Equal(HttpStatusCode.OK, delResp.StatusCode);

        var getResp = await client.GetAsync($"/api/Reviews/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }

    private static async Task<Guid> CreateUserAsync(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = new User(
            "Test",
            "User",
            Email.Create($"test{Guid.NewGuid():N}@example.com"),
            PasswordHash.Create("hash", "salt"),
            UserRole.User);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user.Id;
    }
}
