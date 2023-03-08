using Domain.Objects;

namespace Application.Interfaces;

/*
Collection-repository interface: 
    > No parts of the persistence mechanisms are surfaced to the client by its public interface.
    > They mimic the behavior of Collections (Hashset).
*/
public interface IRepository<TEntity>
    where TEntity : IAggregateRoot
{
    // Write Model
    Task AddAsync(TEntity entity);
    Task<TEntity> GetByIdAsync(int id);
    Task RemoveAsync(TEntity entity);
    // Read Model - method returns IQueryable<TEntity> - read-only
    //IQueryable<TEntity> AsQueryable();
}
