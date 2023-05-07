using System.Net;

using FluentResults;

namespace Domain.Common.ErrorTypes;
public interface IProblemDetailsError : IError, IStatusCode
{
    string Title { get; init; }
}
