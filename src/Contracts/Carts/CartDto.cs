using System.Text.Json.Serialization;

using Domain.Carts;

namespace Presentation.Contracts.Carts;

public sealed record CartDto(Guid Id,
    HashSet<CartItemDto> Items,
    bool IsPersisted,
    int ItemsCount,
    decimal TotalPrice)
{
    /// <summary>
    /// SetItem the Cart as persisted in the Store.
    /// </summary>
    public CartDto AsPersisted() => this with { IsPersisted = true };

    /// <summary>
    /// Returns true if the Cart has been persisted in the Store or if it has no items.
    /// </summary>
    [JsonIgnore]
    public bool IsNotPersistedAndHasItems => !IsPersisted && Items.Any();

    /// <summary>
    /// Creates a CartDto with a new Guid and an empty HashSet of CartItemDto. isPersisted is false by default.
    /// </summary>
    /// <returns></returns>
    public static CartDto Create()
    {
        HashSet<CartItemDto> items = new();
        decimal totalPrice = items.Aggregate(0m, (acc, item) => acc + item.UnitPrice);
        return new(Guid.Empty, items, IsPersisted: false, items.Count, totalPrice);
    }

    //public void SetItem(CartItemDto cartItem)
    //{
    //    if(Items.TryGetValue(cartItem, out var item))
    //    {
    //        item.UpdateQuantity(cartItem.Quantity);
    //    }
    //    else
    //    {
    //        Items.Add(cartItem);
    //    }
    //}

    public CartDto SetItem(CartItemDto cartItem)
    {
        Items.Remove(cartItem);
        Items.Add(cartItem);

        return this with
        {
            Items = Items,
            ItemsCount = Items.Aggregate(0,  (acc, item) => acc + item.Quantity),
            TotalPrice = Items.Aggregate(0m, (acc, item) => acc + item.UnitPrice * item.Quantity)
        };
    }

    public CartDto RemoveItem(CartItemDto cartItem)
    {
        Items.Remove(cartItem);

        return this with
        {
            Items = Items,
            ItemsCount = Items.Aggregate(0,  (acc, item) => acc + item.Quantity),
            TotalPrice = Items.Aggregate(0m, (acc, item) => acc + item.UnitPrice * item.Quantity)
        };
    }

    public CartDto MapWith(Cart cart)
    {
        return this with
        {
            Id = cart.Id.Value,
            Items = cart.Items.Select(item => item.MapTo()).ToHashSet()
        };
    }
}