﻿using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Products;

using FluentResults;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Application.Products.Queries;

public record class GetProductsQuery : IRequest<Result<IEnumerable<Product>>>
{
    internal sealed class Handler : IRequestHandler<GetProductsQuery, Result<IEnumerable<Product>>>

    {
        private readonly IApplicationDbContext _applicationDbContext;
        public Handler(IApplicationDbContext context)
        {
            _applicationDbContext = context;
        }
        public async Task<Result<IEnumerable<Product>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            List<Product> products = await _applicationDbContext.Products.ToListAsync(cancellationToken);
            return products;
        }
    }
}

