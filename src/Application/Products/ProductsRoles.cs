using Application.Common.Security;

namespace Application.Products;
public static class ProductsRoles
{
    //Roles
    public const string ProductManager = nameof(ProductManager);
    public const string ProductAdmin = nameof(ProductAdmin);
}

public static class ProductViewPolicy
{
    public const string PolicyName = nameof(ProductViewPolicy);
    public static readonly IReadOnlyList<string> Roles = new List<string>() { ProductsRoles.ProductAdmin, ProductsRoles.ProductManager, VisitorRoles.Visitor};
}

public static class ProductManagementPolicy
{
    public const string PolicyName = nameof(ProductManagementPolicy);
    public static readonly IReadOnlyList<string> Roles = new List<string>() { ProductsRoles.ProductAdmin, ProductsRoles.ProductManager };
}

public static class ProductAdminPolicy
{
    public const string PolicyName = nameof(ProductAdminPolicy);
    public static readonly IReadOnlyList<string> Roles = new List<string>() { ProductsRoles.ProductAdmin };
}
