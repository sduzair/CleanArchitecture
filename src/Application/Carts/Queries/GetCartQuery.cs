using Application.Common.Security;
using Application.Common.Security.Policies;

using Domain.Carts;

using FluentResults;

using MediatR;

namespace Application.Carts.Queries;

[ApplicationAuthorize(Policy = nameof(CartPolicy))]
public record GetCartQuery : IRequest<Result<Cart>>;