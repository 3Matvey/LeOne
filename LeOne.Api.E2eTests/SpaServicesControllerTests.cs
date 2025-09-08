using System.Net;
using System.Net.Http.Json;
using LeOne.Application.SpaServices.Commands.CreateSpaService;
using LeOne.Application.SpaServices.Dtos;
using LeOne.Application.SpaServices.Queries.ListSpaService;
using Xunit;

namespace LeOne.API.E2ETests;


[Collection("E2E Database")]
public class SpaServicesControllerTests
{
    private record CreateSpaServiceResponse(string Message, SpaServiceDto SpaServiceDto);

    [Fact]
    public async Task Create_ReturnsCreatedSpaService()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var cmd = new CreateSpaServiceCommand("Massage", 1000, 60, "Relax");
        var response = await client.PostAsJsonAsync("/api/SpaServices", cmd);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateSpaServiceResponse>();
        Assert.NotNull(result);
        Assert.Equal(cmd.Name, result!.SpaServiceDto.Name);
        Assert.Equal(cmd.PriceInCents, result.SpaServiceDto.PriceInCents);
    }

    [Fact]
    public async Task GetById_ReturnsSpaService()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var cmd = new CreateSpaServiceCommand("Facial", 2000, 30, "Face");
        var createResp = await client.PostAsJsonAsync("/api/SpaServices", cmd);
        var created = await createResp.Content.ReadFromJsonAsync<CreateSpaServiceResponse>();
        var id = created!.SpaServiceDto.Id;

        var getResp = await client.GetAsync($"/api/SpaServices/{id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
        var dto = await getResp.Content.ReadFromJsonAsync<SpaServiceDto>();
        Assert.NotNull(dto);
        Assert.Equal(cmd.Name, dto!.Name);
    }

    [Fact]
    public async Task List_ReturnsPaginatedResults()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        // create three services
        await client.PostAsJsonAsync("/api/SpaServices", new CreateSpaServiceCommand("S1", 100, 10, null));
        await client.PostAsJsonAsync("/api/SpaServices", new CreateSpaServiceCommand("S2", 200, 20, null));
        await client.PostAsJsonAsync("/api/SpaServices", new CreateSpaServiceCommand("S3", 300, 30, null));

        var listResp = await client.GetAsync("/api/SpaServices?page=2&pageSize=2");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);
        var body = await listResp.Content.ReadFromJsonAsync<ListSpaServiceResponse>();
        Assert.NotNull(body);
        Assert.Equal(3, body!.TotalCount);
        Assert.Equal(2, body.Page);
        var item = Assert.Single(body.Items);
        Assert.Equal("S3", item.Name);
    }

    [Fact]
    public async Task ChangePrice_UpdatesSpaService()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var createCmd = new CreateSpaServiceCommand("Pedicure", 500, 20, null);
        var createResp = await client.PostAsJsonAsync("/api/SpaServices", createCmd);
        var created = await createResp.Content.ReadFromJsonAsync<CreateSpaServiceResponse>();
        var id = created!.SpaServiceDto.Id;

        var patchResp = await client.PatchAsJsonAsync($"/api/SpaServices/{id}/price", new { newPriceInCents = 750L });
        Assert.Equal(HttpStatusCode.OK, patchResp.StatusCode);
        var updated = await patchResp.Content.ReadFromJsonAsync<SpaServiceDto>();
        Assert.NotNull(updated);
        Assert.Equal(750L, updated!.PriceInCents);
    }

    [Fact]
    public async Task Delete_RemovesSpaService()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var cmd = new CreateSpaServiceCommand("Manicure", 400, 15, null);
        var createResp = await client.PostAsJsonAsync("/api/SpaServices", cmd);
        var created = await createResp.Content.ReadFromJsonAsync<CreateSpaServiceResponse>();
        var id = created!.SpaServiceDto.Id;

        var delResp = await client.DeleteAsync($"/api/SpaServices/{id}");
        Assert.Equal(HttpStatusCode.OK, delResp.StatusCode);

        var getResp = await client.GetAsync($"/api/SpaServices/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }
}