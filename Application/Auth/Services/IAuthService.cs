using TodoAppAPI.Application.Auth.Commands;
using TodoAppAPI.Application.Auth.Models;

namespace TodoAppAPI.Application.Auth.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginUserCommand command, CancellationToken cancellationToken = default);
}
