using Domain.Carts.Entities;
using Domain.Common.ErrorTypes;

using FluentResults;

namespace Domain.Carts.Errors;

public class MustNotBeZeroTotalQuantityValidationError : Error, IValidationError
{
    public MustNotBeZeroTotalQuantityValidationError()
    {
        Message = "Total quantity cannot be zero";
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