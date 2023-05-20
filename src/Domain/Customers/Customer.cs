using Domain.Carts;
using Domain.Common;
using Domain.Customers.ValueObjects;

namespace Domain.Customers;

public class Customer : AggregateRoot<CustomerId>
{
    //foreign key to ApplicationUser
    public Guid ApplicationUserId { get; init; }

    //TODO - check if lazy loading works when Cart is not nullable or when init is used
    //navigation property to Cart (one-to-one)
    public Cart? Cart { get; private set; }
    ///TODO - can we use this instead, it's better because it does not allow methods like <see cref="ReplaceCart"/> to be called but will this work with lazy loading?
    //public Cart? Cart { get; init; }

    //TODO - does this replace the cart and delete the old one when SaveChanges is called?
    //public void ReplaceCart()
    //{
    //    Cart = Cart.Create(Id);
    //}

    //ef core constructor to map entity
    private Customer() { }

    public static Customer Create(Guid applicationUserId)
    {
        return new Customer() { Id = CustomerId.Create(), ApplicationUserId = applicationUserId };
    }
}