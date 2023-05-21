using Domain.Carts.Entities;
using Domain.Carts.Errors;
using Domain.Carts.ValueObjects;
using Domain.Common;
using Domain.Customers.ValueObjects;

using FluentResults;

namespace Domain.Carts;

public sealed class Cart : AggregateRoot<CartId>
{
    //TODO check lazy loading
    //backing field for EF Core
    private readonly HashSet<CartItem> _items = new();
    public IReadOnlySet<CartItem> Items { get { return _items; } }

    public CustomerId CustomerId { get; init; }

    //ef core constructor to map entity
    private Cart() { }

    private Cart(CartId id, CustomerId customerId, IEnumerable<CartItem>? items)
    {
        Id = id;
        CustomerId = customerId;
        _items = items == null ? new HashSet<CartItem>() : items.ToHashSet();
    }

    /// <summary>
    /// <paramref name="customerId"/> is optional when the user has a role of <see cref="VisitorRoles.Visitor"/>
    /// </summary>
    /// <param name="customerId">Optional when the user has a role of <see cref="VisitorRoles.Visitor"/></param>
    /// <returns></returns>
    public static Cart Create(CustomerId customerId, IEnumerable<CartItem>? items = default)
    {
        return new(CartId.Create(), customerId, items);
    }

    public Result AddItemOrUpdateQuantity(CartItem cartItemToAddOrUpdate)
    {
        Result result = new();

        if (!MustHavePositiveUnitPrice(cartItemToAddOrUpdate))
        {
            result.WithError(new MustHavePositiveUnitPriceValidationError(cartItemToAddOrUpdate.UnitPrice));
        }

        Result? resultAddOrUpdate;
        if (_items.TryGetValue(cartItemToAddOrUpdate, out var item))
        {
            resultAddOrUpdate = item.UpdateQuantity(cartItemToAddOrUpdate.Quantity);
        }
        else
        {
            resultAddOrUpdate = AddItem(cartItemToAddOrUpdate);
        }

        if (resultAddOrUpdate.IsFailed)
        {
            result.WithErrors(resultAddOrUpdate.Errors);
        }
        return result;
    }

    private static bool MustHavePositiveUnitPrice(CartItem item)
    {
        return item.UnitPrice > 0;
    }

    private Result AddItem(CartItem cartItemToAdd)
    {
        var result = new Result();
        if (!CartItem.MustNotBeZeroQuantity(0, cartItemToAdd.Quantity))
        {
            result.WithError(new MustNotBeZeroQuantityValidationError());
        }
        if (!CartItem.MustHavePositiveQuantity(0, cartItemToAdd.Quantity))
        {
            result.WithError(new MustHavePositiveTotalQuantityValidationError(0 +  cartItemToAdd.Quantity));
        }
        if (result.IsFailed)
        {
            return result;
        }

        _items.Add(cartItemToAdd);
        return Result.Ok();
    }

    public Result AddItems(IReadOnlyCollection<CartItem> items)
    {
        var result = new Result();
        for(int i = 0; i < items.Count; i++)
        {
            var resultAddItem = AddItem(items.ElementAt(i));
            if (resultAddItem.IsFailed)
            {
                result.WithErrors(resultAddItem.Errors);
            }
        }
        return result;
    }

    public void RemoveItem(CartItem cartItem)
    {
        _items.Remove(cartItem);
    }
}
