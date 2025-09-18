using LeOne.Application.Auth.Commands.Login;
using LeOne.Application.Auth.Commands.Refresh;
using LeOne.Application.Auth.Commands.Register;
using LeOne.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace LeOne.API.E2ETests;

[Collection("E2E Database")]
public class AuthControllerTests
{
    private record AuthTokensResponse(
        string AccessToken,
        DateTimeOffset AccessTokenExpiresAt,
        string RefreshToken,
        DateTimeOffset RefreshTokenExpiresAt,
        string TokenType
    );

    private static string UniqueEmail(string prefix = "user") =>
        $"{prefix}-{Guid.NewGuid():N}@test.local";
    private static async Task<(HttpResponseMessage Response, string Email, string Password)> RegisterUser(
        HttpClient client,
        string prefix,
        string firstName = "John",
        string lastName = "Doe",
        string password = "S3curE!Pass")
    {
        var email = UniqueEmail(prefix);
        var cmd = new RegisterUserCommand(firstName, lastName, email, password);
        var response = await client.PostAsJsonAsync("/api/Auth/register", cmd);
        return (response, email, password);
    }

    private static async Task<AuthTokensResponse> ValidateAuthTokens(
        HttpResponseMessage response,
        bool validateExpiration = true)
    {
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokens = await response.Content.ReadFromJsonAsync<AuthTokensResponse>();

        Assert.NotNull(tokens);
        Assert.False(string.IsNullOrWhiteSpace(tokens!.AccessToken));
        Assert.False(string.IsNullOrWhiteSpace(tokens.RefreshToken));
        Assert.Equal("Bearer", tokens.TokenType);

        if (validateExpiration)
        {
            Assert.True(tokens.AccessTokenExpiresAt > DateTimeOffset.UtcNow);
            Assert.True(tokens.RefreshTokenExpiresAt > DateTimeOffset.UtcNow);
        }

        return tokens;
    }

    [Fact]
    public async Task Register_ReturnsAuthTokens()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var (response, _, _) = await RegisterUser(client, "reg");
        await ValidateAuthTokens(response);
    }

    [Fact]
    public async Task Register_ReturnsConflict_WhenUserExists()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var (first, email, password) = await RegisterUser(client, "dup");
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var cmd = new RegisterUserCommand("Jane", "Doe", email, password);
        var second = await client.PostAsJsonAsync("/api/Auth/register", cmd);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsAuthTokens_WhenCredentialsValid()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var (regResponse, email, password) = await RegisterUser(client, "login-ok", "Tim", "Taylor", "S3curE!Pass1");
        Assert.Equal(HttpStatusCode.OK, regResponse.StatusCode);

        var loginCmd = new LoginUserCommand(email, password);
        var loginResp = await client.PostAsJsonAsync("/api/Auth/login", loginCmd);
        await ValidateAuthTokens(loginResp);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var (regResponse, email, _) = await RegisterUser(client, "login-bad", "Lia", "Smith", "Valid#Pass2");
        Assert.Equal(HttpStatusCode.OK, regResponse.StatusCode);

        var badLogin = new LoginUserCommand(email, "WrongPassword!");
        var loginResp = await client.PostAsJsonAsync("/api/Auth/login", badLogin);
        Assert.Equal(HttpStatusCode.Unauthorized, loginResp.StatusCode);
    }

    [Fact]
    public async Task Refresh_ReturnsNewTokens_WhenRefreshIsValid()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var (regResponse, email, password) = await RegisterUser(
            client, "refresh-ok", "Rick", "Deckard", "S3curE!Pass3");
        Assert.Equal(HttpStatusCode.OK, regResponse.StatusCode);

        var loginCmd = new LoginUserCommand(email, password);
        var loginResp = await client.PostAsJsonAsync("/api/Auth/login", loginCmd);
        var initial = await ValidateAuthTokens(loginResp);

        using (var scope = factory.Services.CreateScope())
        {
            var gen = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();
            var (uid, jti) = gen.ValidateRefreshTokenRich(initial.RefreshToken);
            Assert.NotNull(uid);
            Assert.False(string.IsNullOrWhiteSpace(jti));
        }

        var content = JsonContent.Create(new { token = initial.RefreshToken });
        var refreshResp = await client.PostAsync("/api/Auth/refresh", content);
        var refreshed = await ValidateAuthTokens(refreshResp);

        Assert.NotEqual(initial.AccessToken, refreshed.AccessToken);
        Assert.NotEqual(initial.RefreshToken, refreshed.RefreshToken);
    }

    [Fact]
    public async Task Refresh_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        using var factory = new CustomWebApplicationFactory();
        var client = factory.CreateClient();

        var invalid = new RefreshTokenCommand("this.is.not.a.valid.jwt");
        var resp = await client.PostAsJsonAsync("/api/Auth/refresh", invalid);
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }
}
