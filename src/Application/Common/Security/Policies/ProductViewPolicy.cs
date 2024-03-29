﻿using Persistence.Identity.Roles;

namespace Application.Common.Security.Policies;

public static class ProductViewPolicy
{
    public static readonly IReadOnlyList<string> Roles = new List<string>() { nameof(ProductAdmin), nameof(ProductAdmin), nameof(Visitor), nameof(Customer) };
}
