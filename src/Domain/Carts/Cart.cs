using Domain.Carts.Entities;
using Domain.Carts.ValueObjects;
using Domain.Common;
using Domain.Customers.ValueObjects;

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
    //public static Cart Create()
    //{
    //    var customerId = CustomerId.Create();
    //    var items = new HashSet<CartItem>();
    //    return new(CartId.Create(), customerId, items);
    //}

    public void UpdateItemQuantityOrAddItem(CartItem cartItem)
    {
        //var cartItem = CartItem.From(CartItemId.From(Items.Count + 1), Id, product);
        if (_items.TryGetValue(cartItem, out var item))
        {
            item.UpdateQuantity(cartItem.Quantity);
        }
        else
        {
            _items.Add(cartItem);
        }
    }

    public void AddItems(IReadOnlyCollection<CartItem> items)
    {
        for(int i = 0; i < items.Count; i++)
        {
            UpdateItemQuantityOrAddItem(items.ElementAt(i));
        }
    }

    public void RemoveItem(CartItem cartItem)
    {
        _items.Remove(cartItem);
    }
}
