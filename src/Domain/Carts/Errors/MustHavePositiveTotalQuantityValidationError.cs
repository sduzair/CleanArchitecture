using System.Collections.Generic;

using Domain.Carts.Entities;
using Domain.Common.ErrorTypes;

using FluentResults;

namespace Domain.Carts.Errors;
public class MustHavePositiveTotalQuantityValidationError : Error, IValidationError
{
    public MustHavePositiveTotalQuantityValidationError(int newTotalQuantity)
    {
        Message = $"Quantity must be positive. Quantity: {newTotalQuantity}";
        Metadata.Add("Key", nameof(CartItem.Quantity));
    }

    public string Key
    {
        get
        {
            if (Metadata.TryGetValue("Key", out var keyName))
            {
                return (string)keyName;
            }
            return "SomeKey";
        }
    }
}
