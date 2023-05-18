using Domain.Carts.Entities;
using Domain.Carts.ValueObjects;

using FluentResults;

using MediatR;

namespace Application.Carts.Commands;
public record AddCartItemsCommand(CartId CartId, HashSet<CartItem> CartItems) : IRequest<Result>;
