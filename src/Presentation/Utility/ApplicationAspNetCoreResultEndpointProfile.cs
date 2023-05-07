using System.Net;

using Domain.Common.ErrorTypes;

using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Presentation.Utility;
public class ApplicationAspNetCoreResultEndpointProfile : DefaultAspNetCoreResultEndpointProfile
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public ApplicationAspNetCoreResultEndpointProfile(ProblemDetailsFactory problemDetailsFactory, IHttpContextAccessor contextAccessor)
    {
        _problemDetailsFactory = problemDetailsFactory;
        _contextAccessor = contextAccessor;
    }

    public override ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
    {
        IProblemDetailsError? statusCodeError = context.Result.Errors.OfType<IProblemDetailsError>().FirstOrDefault();

        if (statusCodeError == null)
        {
            var internalServerProblemDetails = _problemDetailsFactory.CreateProblemDetails(
                httpContext: _contextAccessor.HttpContext!);

            internalServerProblemDetails.Extensions.Add("reasons", context.Result.Reasons.Select(reason => reason.Message));

            return new ObjectResult(internalServerProblemDetails)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        var problemDetails = _problemDetailsFactory.CreateProblemDetails(
            httpContext: _contextAccessor.HttpContext!,
            statusCode: statusCodeError.StatusCode,
            title: statusCodeError.Title,
            detail: statusCodeError.Message);

        problemDetails.Extensions.Add("reasons", context.Result.Reasons.Select(reason => reason.Message));

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCodeError.StatusCode
        };
    }
}
