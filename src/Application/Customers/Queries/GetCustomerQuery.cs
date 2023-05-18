using Domain.Customers;

using FluentResults;

using MediatR;

namespace Application.Customers.Queries;

/// <summary>
/// This query is used to get a customer by their application user id.
/// </summary>
/// <param name="ApplicaitonUserId">This the foreign key to the Users table</param>
public record GetCustomerQuery(Guid ApplicaitonUserId) : IRequest<Result<Customer>>;
