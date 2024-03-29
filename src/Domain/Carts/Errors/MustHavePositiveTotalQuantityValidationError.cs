﻿using System.Collections.Generic;

using Domain.Carts.Entities;
using Domain.Common.ErrorTypes;

using FluentResults;

namespace Domain.Carts.Errors;
public class MustHavePositiveTotalQuantityValidationError : Error, IValidationError
{
    public MustHavePositiveTotalQuantityValidationError()
    {
        Message = $"Total quantity must be positive.";
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
