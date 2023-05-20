using Application.Common.Security.Roles;

namespace Application.Common.Security.Policies;

public static class ProductsViewPolicy
{
    public static readonly IReadOnlyList<string> Roles = new List<string>() { nameof(ProductAdmin), nameof(ProductAdmin), nameof(Visitor), nameof(Customer) };
}
