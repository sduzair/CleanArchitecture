using Application.Common.Security.Roles;

namespace Application.Common.Security.Policies;

public static class ProductsManagementPolicy
{
    public static readonly IReadOnlyList<string> Roles = new List<string>() { nameof(Visitor), nameof(ProductManager), nameof(ProductAdmin) };
}
