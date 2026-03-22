using TodoAppAPI.Domain.Entities;

namespace TodoAppAPI.Application.Common.Interfaces;

public interface ITokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateToken(User user);
}
