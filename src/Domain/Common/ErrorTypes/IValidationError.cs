using FluentResults;

namespace Domain.Common.ErrorTypes;

public interface IValidationError : IError
{
    string Key { get; }
}
