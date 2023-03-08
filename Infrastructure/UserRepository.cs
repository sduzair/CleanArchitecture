using Application.Interfaces;

using Domain.Entities;

namespace Infrastructure;
internal sealed class UserRepository : IUserRepository<User>, IRepository<User>
{
    private static readonly List<User> _users = new();
    public Task AddAsync(User entity)
    {
        _users.Add(entity);
        return Task.CompletedTask;
    }

    public Task<User> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByEmail(string email)
    {
        return Task.FromResult(_users.SingleOrDefault(x => x.Email == email));
    }

    public Task RemoveAsync(User entity)
    {
        throw new NotImplementedException();
    }
}
