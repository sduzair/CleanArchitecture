using System.Security.Claims;

using Application.Customers.Commands;
using Application.Customers.Queries;

using FluentResults.Extensions;
using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Presentation.Common;
using Presentation.Contracts.Customers;

namespace Presentation.Customers;

[Authorize]
public sealed class CustomerController : ApiControllerBase
{
    private readonly ApplicationAspNetCoreResultEndpointProfile _resultProfile;

    public CustomerController(ApplicationAspNetCoreResultEndpointProfile resultProfile)
    {
        _resultProfile = resultProfile;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer()
    {
        var userId = GetUserIdFromClaimsPrinciple();
        var result = await Mediator.Send(new CreateCustomerCommand(userId));
        if (result.IsFailed)
        {
            return result.ToActionResult(_resultProfile);
        }
        return CreatedAtAction(nameof(GetCustomer), new { id = result.Value.ToString() }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomer()
    {
        var userId = GetUserIdFromClaimsPrinciple();
        return await Mediator.Send(new GetCustomerByApplicationUserIdQuery(userId))
            .Map(customer => customer.MapTo())
            .ToActionResult(_resultProfile);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCustomer()
    {
        var userId = GetUserIdFromClaimsPrinciple();
        return await Mediator.Send(new DeleteCustomerCommand(userId))
            .ToActionResult(_resultProfile);
    }

    private Guid GetUserIdFromClaimsPrinciple()
    {
        return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
