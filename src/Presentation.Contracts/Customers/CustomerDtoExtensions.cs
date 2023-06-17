using Domain.Customers;

namespace Presentation.Contracts.Customers;

public static class CustomerDtoExtensions
{
    public static CustomerDto MapTo(this Customer customer)
    {
        return new CustomerDto
        (
            Id: customer.Id.Value,
            ApplicationUserId: customer.ApplicationUserId
            //Name = customer.Name,
            //Email = customer.Email,
            //Phone = customer.Phone,
            //Address = customer.Address,
            //City = customer.City,
            //Country = customer.Country,
            //ZipCode = customer.ZipCode
        );
    }
}
