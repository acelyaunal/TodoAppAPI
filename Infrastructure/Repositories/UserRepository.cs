using Microsoft.EntityFrameworkCore;
using TodoAppAPI.Application.Interfaces.Repositories;
using TodoAppAPI.Domain.Entities;
using TodoAppAPI.Infrastructure.Data;

namespace TodoAppAPI.Infrastructure.Repositories;

public sealed class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return dbContext.Users.FirstOrDefaultAsync(x => x.Email == normalizedEmail, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }
}
