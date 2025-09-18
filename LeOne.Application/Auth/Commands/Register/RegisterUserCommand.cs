namespace LeOne.Application.Auth.Commands.Register;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password);