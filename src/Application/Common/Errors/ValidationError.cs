using Domain.Common.ErrorTypes;

using FluentResults;

namespace Application.Common.Errors;

internal class ValidationError : Error, IValidationError
{
    public ValidationError(IDictionary<string, string[]> validationDictionary)
    {
        _validationDictionary = validationDictionary;
    }
    private readonly IDictionary<string, string[]> _validationDictionary;

    public IReadOnlyDictionary<string, string[]> GetValidationDictionary()
    {
        return _validationDictionary.AsReadOnly();
    }
}