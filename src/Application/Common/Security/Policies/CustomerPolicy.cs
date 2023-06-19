using Persistence.Identity.Roles;

namespace Application.Common.Security.Policies;
public static class CustomerPolicy
{
    public static readonly IReadOnlyList<string> Roles = new List<string>() { nameof(Customer) };
}
