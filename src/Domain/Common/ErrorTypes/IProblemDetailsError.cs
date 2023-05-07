using System.Net;

using FluentResults;

namespace Domain.Common.ErrorTypes;
public interface IProblemDetailsError : IReason
{
    int StatusCode { get; init; }
    string Title { get; init; }
}
