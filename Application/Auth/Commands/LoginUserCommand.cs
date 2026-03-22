namespace TodoAppAPI.Application.Auth.Commands;

public sealed record LoginUserCommand(string Email, string Password);
