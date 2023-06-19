using Persistence.Identity.Roles;

namespace Application.Common.Security.Policies;

public static class ProductManagementPolicy
{
    public static readonly IReadOnlyList<string> Roles = new List<string>() { nameof(ProductManager), nameof(ProductAdmin) };
}
