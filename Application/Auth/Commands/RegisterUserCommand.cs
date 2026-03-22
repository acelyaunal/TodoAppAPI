namespace TodoAppAPI.Application.Auth.Commands;

public sealed record RegisterUserCommand(string Email, string Password);
