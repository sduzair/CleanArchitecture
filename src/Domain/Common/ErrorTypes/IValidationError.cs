using FluentResults;

namespace Domain.Common.ErrorTypes;

public interface IValidationError : IError
{
    IReadOnlyDictionary<string, string[]> GetValidationDictionary();
}
