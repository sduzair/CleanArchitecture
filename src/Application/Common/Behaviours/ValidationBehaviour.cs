using Application.Common.Errors;

using FluentResults;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace Application.Common.Behaviours;

internal sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : ResultBase, new()
{
    private readonly IValidator<TRequest>? _validator;

    public ValidationBehaviour(IValidator<TRequest>? validator = null)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validator == null)
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        ValidationResult validationResult = await _validator.ValidateAsync(context, cancellationToken);

        if (!validationResult.IsValid)
        {
            IDictionary<string, string[]> validationDictionary = validationResult.ToDictionary();
            var result = new TResponse();
            result.Reasons.Add(new ValidationError(validationDictionary));
            return result;
        }

        return await next();
    }
}
