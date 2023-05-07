using System.Net;

using Domain.Common.ErrorTypes;

using FluentResults;
using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        IValidationError? validationError = context.Result.Errors.OfType<IValidationError>().FirstOrDefault();

        if (validationError != null)
        {
            var modelState = new ModelStateDictionary();
            foreach (var entry in validationError.GetValidationDictionary())
            {
                foreach (var value in entry.Value)
                {
                    modelState.AddModelError(entry.Key, value);
                }
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
