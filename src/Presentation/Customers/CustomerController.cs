﻿using System.Security.Claims;

using Application.Common.Security.Policies;
using Application.Customers.Commands;
using Application.Customers.Queries;

using FluentResults.Extensions;
using FluentResults.Extensions.AspNetCore;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Presentation.Common;

namespace Presentation.Customers;

public sealed class CustomerController : ApiControllerBase
{
    private readonly CustomAspNetCoreResultEndpointProfile _resultProfile;

    public CustomerController(CustomAspNetCoreResultEndpointProfile resultProfile)
    {
        _resultProfile = resultProfile;
    }

    [HttpPost]
    [Authorize(Policy = nameof(CustomerPolicy))]
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
    [Authorize(Policy = nameof(CustomerPolicy))]
    public async Task<IActionResult> GetCustomer()
    {
        var userId = GetUserIdFromClaimsPrinciple();
        return await Mediator.Send(new GetCustomerByApplicationUserIdQuery(userId))
            .Map(customer => customer.MapTo())
            .ToActionResult(_resultProfile);
    }

    [HttpDelete]
    [Authorize(Policy = nameof(CustomerPolicy))]
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
