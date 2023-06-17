using Application.Common.Security.Roles;

namespace Application.Common.Security.Policies;

public static class ProductAdminPolicy
{
    public static readonly IReadOnlyList<string> Roles = new List<string>() { nameof(ProductAdmin) };
}
