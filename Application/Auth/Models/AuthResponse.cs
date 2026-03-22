namespace TodoAppAPI.Application.Auth.Models;

public sealed record AuthResponse(
    string Token,
    DateTime ExpiresAtUtc,
    int UserId,
    string Email);
