using Persistence.Identity.Roles;

namespace Application.Common.Security.Policies;

/// <summary>
/// The purpose of this policy is to allow access to the cart to <see cref="Visitor"/> and <see cref="Customer"/> roles.
/// </summary>
public static class CartPolicy
{
    public static readonly IReadOnlyList<string> Roles = new List<string>() { nameof(Visitor), nameof(Customer) };
}