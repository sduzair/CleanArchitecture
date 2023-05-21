using Domain.Carts.Entities;
using Domain.Common.ErrorTypes;

using FluentResults;

namespace Domain.Carts.Errors;
public class MustHavePositiveUnitPriceValidationError : Error, IValidationError
{
    public MustHavePositiveUnitPriceValidationError(decimal unitPrice)
    {
        Message = $"Unit price must be positive. Unit price: {unitPrice}";
        Metadata.Add("Key", nameof(CartItem.UnitPrice));
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
