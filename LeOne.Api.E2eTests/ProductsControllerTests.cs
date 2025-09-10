using System.Net;
using System.Net.Http.Json;
using LeOne.Application.Products.Commands.CreateProduct;
using LeOne.Application.Products.Dtos;
using LeOne.Application.Products.Queries.ListProduct;
using Xunit;

namespace LeOne.API.E2ETests;

[Collection("E2E Database")]
public class ProductsControllerTests
{
    private record CreateProductResponse(string Message, ProductDto SpaServiceDto);

    [Fact]
    public async Task Create_ReturnsCreatedProduct()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var cmd = new CreateProductCommand("Shampoo", 1299, "Hair care");
        var response = await client.PostAsJsonAsync("/api/Products", cmd);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateProductResponse>();
        Assert.NotNull(result);
        Assert.Equal(cmd.Name, result!.SpaServiceDto.Name);
        Assert.Equal(cmd.PriceInCents, result.SpaServiceDto.PriceInCents);
    }

    [Fact]
    public async Task GetById_ReturnsProduct()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var cmd = new CreateProductCommand("Conditioner", 1599, "Hair care");
        var createResp = await client.PostAsJsonAsync("/api/Products", cmd);
        var created = await createResp.Content.ReadFromJsonAsync<CreateProductResponse>();
        var id = created!.SpaServiceDto.Id;

        var getResp = await client.GetAsync($"/api/Products/{id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
        var dto = await getResp.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(dto);
        Assert.Equal(cmd.Name, dto!.Name);
        Assert.Equal(cmd.PriceInCents, dto.PriceInCents);
    }

    [Fact]
    public async Task List_ReturnsPaginatedResults()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var baselineResp = await client.GetAsync("/api/Products?page=1&pageSize=1");
        Assert.Equal(HttpStatusCode.OK, baselineResp.StatusCode);
        var baselineBody = await baselineResp.Content.ReadFromJsonAsync<ListProductResponse>();
        Assert.NotNull(baselineBody);
        var baselineCount = baselineBody!.TotalCount;

        await client.PostAsJsonAsync("/api/Products", new CreateProductCommand("P1", 100, null));
        await client.PostAsJsonAsync("/api/Products", new CreateProductCommand("P2", 200, null));
        await client.PostAsJsonAsync("/api/Products", new CreateProductCommand("P3", 300, null));

        var expectedTotal = baselineCount + 3;

        var lastPage = expectedTotal;
        var listResp = await client.GetAsync($"/api/Products?page={lastPage}&pageSize=1");
        Assert.Equal(HttpStatusCode.OK, listResp.StatusCode);

        var body = await listResp.Content.ReadFromJsonAsync<ListProductResponse>();
        Assert.NotNull(body);
        Assert.Equal(expectedTotal, body!.TotalCount);
        Assert.Equal(lastPage, body.Page);

        var item = Assert.Single(body.Items);
        Assert.Equal("P3", item.Name);
    }


    [Fact]
    public async Task ChangePrice_UpdatesProduct()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var createCmd = new CreateProductCommand("Body Lotion", 799, null);
        var createResp = await client.PostAsJsonAsync("/api/Products", createCmd);
        var created = await createResp.Content.ReadFromJsonAsync<CreateProductResponse>();
        var id = created!.SpaServiceDto.Id;

        var patchResp = await client.PatchAsJsonAsync($"/api/Products/{id}/price", new { newPriceInCents = 999L });
        Assert.Equal(HttpStatusCode.OK, patchResp.StatusCode);
        var updated = await patchResp.Content.ReadFromJsonAsync<ProductDto>();
        Assert.NotNull(updated);
        Assert.Equal(999L, updated!.PriceInCents);
    }

    [Fact]
    public async Task Delete_RemovesProduct()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var cmd = new CreateProductCommand("Bath Salt", 499, null);
        var createResp = await client.PostAsJsonAsync("/api/Products", cmd);
        var created = await createResp.Content.ReadFromJsonAsync<CreateProductResponse>();
        var id = created!.SpaServiceDto.Id;

        var delResp = await client.DeleteAsync($"/api/Products/{id}");
        Assert.Equal(HttpStatusCode.OK, delResp.StatusCode);

        var getResp = await client.GetAsync($"/api/Products/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }
}
