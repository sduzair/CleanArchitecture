using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Presentation.Filters;

namespace Presentation;

[ApiController]
[Route("api/[controller]/[action]")]
[TypeFilter(typeof(AuthExceptionHandlingFilter))]
public class ApiControllerBase : ControllerBase
{
    private ISender? _meditor;
    protected ISender Mediator => _meditor ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
