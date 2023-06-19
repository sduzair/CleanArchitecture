using System.Text.Json.Serialization;

using Domain.Carts;

namespace Presentation.Carts;

/// <summary>
/// Creates a CartDto with a new Guid and an empty HashSet of CartItemDto. isPersisted is true by default and set to false when user other than <see cref="Customer"/> adds/removes items from the cart.
/// </summary>
public sealed record CartDto(Guid Id,
    HashSet<CartItemDto> Items,
    int ItemsCount,
    decimal TotalPrice,
    bool IsPersisted = true)
{
    /// <summary>
    /// Set the Cart as persisted in the Store.
    /// </summary>
    public CartDto AsPersisted() => this with { IsPersisted = true };

    ///// <summary>
    ///// Set the Cart as not persisted in the Store.
    ///// </summary>
    ///// <returns></returns>
    //public CartDto AsNotPersisted() => this with { IsPersisted = false };

    /// <summary>
    /// Returns true if the Cart has been persisted in the Store or if it has no items.
    /// </summary>
    [JsonIgnore]
    public bool IsNotPersistedAndHasItems => !IsPersisted && Items.Any();

    public static CartDto Create()
    {
        HashSet<CartItemDto> items = new();
        decimal totalPrice = items.Aggregate(0m, (acc, item) => acc + item.UnitPrice);
        return new(Guid.Empty, items, items.Count, totalPrice);
    }

    public CartDto AddItemOrUpdateQuantity(CartItemDto cartItem)
    {
        if (Items.TryGetValue(cartItem, out var item))
        {
            item.UpdateQuantity(cartItem.Quantity);
        }
        else
        {
            Items.Add(cartItem);
        }

        return this with
        {
            Items = Items,
            ItemsCount = Items.Aggregate(0, (acc, item) => acc + item.Quantity),
            TotalPrice = Items.Aggregate(0m, (acc, item) => acc + item.UnitPrice * item.Quantity)
        };
    }

    public CartDto RemoveItem(CartItemDto cartItem)
    {
        var items = Items.ToHashSet();
        items.Remove(cartItem);

        return this with
        {
            Items = items,
            ItemsCount = items.Aggregate(0, (acc, item) => acc + item.Quantity),
            TotalPrice = items.Aggregate(0m, (acc, item) => acc + item.UnitPrice * item.Quantity)
        };
    }

    public CartDto UpdateWith(Cart cart)
    {
        return this with
        {
            Id = cart.Id.Value,
            Items = cart.Items.Select(item => item.MapToDto()).ToHashSet(),
            ItemsCount = cart.Items.Aggregate(0, (acc, item) => acc + item.Quantity),
            TotalPrice = cart.Items.Aggregate(0m, (acc, item) => acc + item.UnitPrice * item.Quantity)
        };
    }
}