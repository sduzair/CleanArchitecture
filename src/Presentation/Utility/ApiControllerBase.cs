using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Presentation.Authentication;

namespace Presentation.Utility;

[ApiController]
[Route("api/[controller]/[action]")]
[TypeFilter(typeof(ExceptionsFilter))]
public class ApiControllerBase : ControllerBase
{
    private ISender? _meditor;
    protected ISender Mediator => _meditor ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
