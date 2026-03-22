using Microsoft.Extensions.Logging;
using TodoAppAPI.Application.Auth.Commands;
using TodoAppAPI.Application.Auth.Models;
using TodoAppAPI.Application.Common.Exceptions;
using TodoAppAPI.Application.Common.Interfaces;
using TodoAppAPI.Application.Interfaces.Repositories;
using TodoAppAPI.Domain.Entities;

namespace TodoAppAPI.Application.Auth.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider,
    ILogger<AuthService> logger) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = User.NormalizeEmail(command.Email);
        var existingUser = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var passwordHash = passwordHasher.HashPassword(command.Password);
        var user = User.Create(command.Email, passwordHash, timeProvider.GetUtcNow().UtcDateTime);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Registered user {UserId}", user.Id);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = User.NormalizeEmail(command.Email);
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!passwordHasher.VerifyPassword(user.PasswordHash, command.Password))
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        logger.LogInformation("Authenticated user {UserId}", user.Id);

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var (token, expiresAtUtc) = tokenGenerator.GenerateToken(user);

        return new AuthResponse(token, expiresAtUtc, user.Id, user.Email);
    }
}
