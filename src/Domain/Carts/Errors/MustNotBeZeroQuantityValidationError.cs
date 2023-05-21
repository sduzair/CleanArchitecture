using Domain.Carts.Entities;
using Domain.Common.ErrorTypes;

using FluentResults;

namespace Domain.Carts.Errors;

public class MustNotBeZeroQuantityValidationError : Error, IValidationError
{
    public MustNotBeZeroQuantityValidationError()
    {
        Message = "Quantity must not be zero";
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