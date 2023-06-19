using System.Net;

using Domain.Common.ErrorTypes;

using FluentResults;
using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation.Common;
public class CustomAspNetCoreResultEndpointProfile : DefaultAspNetCoreResultEndpointProfile
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public CustomAspNetCoreResultEndpointProfile(ProblemDetailsFactory problemDetailsFactory, IHttpContextAccessor contextAccessor)
    {
        _problemDetailsFactory = problemDetailsFactory;
        _contextAccessor = contextAccessor;
    }

    public override ActionResult TransformFailedResultToActionResult(FailedResultToActionResultTransformationContext context)
    {
        //Problem details error
        IProblemDetailsError? statusCodeError = context.Result.Errors.OfType<IProblemDetailsError>().FirstOrDefault();

        if (statusCodeError != null)
        {
            var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                httpContext: _contextAccessor.HttpContext!,
                statusCode: statusCodeError.StatusCode,
                title: statusCodeError.Title,
                detail: statusCodeError.Message);

            if (context.Result.Reasons.Count > 1)
            {
                problemDetails.Extensions.Add("reasons", context.Result.Reasons.Select(reason => reason.Message));
            }

            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCodeError.StatusCode
            };
        }

        //Validation problem details error
        var validationErrors = context.Result.Errors.OfType<IValidationError>();

        if (validationErrors.Any())
        {
            var modelState = new ModelStateDictionary();

            foreach (var validationError in validationErrors)
            {
                modelState.AddModelError(validationError.Key, validationError.Message);
            }

            var validationProblemDetails = _problemDetailsFactory.CreateValidationProblemDetails(
                httpContext: _contextAccessor.HttpContext!,
                modelStateDictionary: modelState);

            return new ObjectResult(validationProblemDetails);
        }

        //Other errors
        var internalServerProblemDetails = _problemDetailsFactory.CreateProblemDetails(
            httpContext: _contextAccessor.HttpContext!);

        internalServerProblemDetails.Extensions.Add("reasons", context.Result.Reasons.Select(reason => reason.Message));

        return new ObjectResult(internalServerProblemDetails)
        {
            StatusCode = (int)HttpStatusCode.InternalServerError
        };
    }
}
