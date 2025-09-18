namespace LeOne.Application.Auth.Commands.Login;

public sealed record LoginUserCommand(
    string Email,
    string Password);