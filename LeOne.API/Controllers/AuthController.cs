using LeOne.Application.Auth.Commands.Login;
using LeOne.Application.Auth.Commands.Register;
using LeOne.Application.Auth.Commands.Refresh;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeOne.Application.Common.Results;

namespace LeOne.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IRegisterUser register, ILoginUser login, IRefreshToken refresher) : ControllerBaseWithResult
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand request, CancellationToken ct)
    {
        var result = await register.HandleAsync(request, ct);

        return result.Match(Ok, Problem);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand request, CancellationToken ct)
    {
        var result = await login.HandleAsync(request, ct);

        return result.Match(Ok, Problem);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand request, CancellationToken ct)
    {
        var result = await refresher.HandleAsync(request, ct);

        return result.Match(Ok, Problem);
    }
}