using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Common;
public abstract class Entity<TId> 
{
    public TId? Id { get; protected init; }
}
