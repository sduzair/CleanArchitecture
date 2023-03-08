using Domain.Objects;

namespace Application.Interfaces;
public interface IUserRepository<TUser> : IRepository<TUser>
    where TUser : IAggregateRoot
{
    Task<TUser?> GetUserByEmail(string email);
}
