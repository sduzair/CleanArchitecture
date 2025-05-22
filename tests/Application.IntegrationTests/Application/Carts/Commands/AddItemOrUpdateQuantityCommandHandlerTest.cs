using Application.Carts.Commands;
using Application.Carts.Queries;
using Application.IntegrationTests;

using Domain.Carts.Entities;
using Domain.Carts.ValueObjects;
using Domain.Customers.ValueObjects;
using Domain.Products.ValueObjects;

using FluentAssertions;

using Persistence.Identity.Roles;

using Presentation.Authentication;

using WebApp.IntegrationTests.Fixtures;

namespace WebApp.IntegrationTests.Application.Carts.Commands;

[Collection(nameof(TestDatabaseCollection))]
public class AddItemOrUpdateQuantityCommandHandlerTest
{
    private readonly TestDatabaseFixture _testDatabaseFixture;

    public AddItemOrUpdateQuantityCommandHandlerTest(TestDatabaseFixture testDatabaseFixture)
    {
        _testDatabaseFixture = testDatabaseFixture;
    }

    [Fact]
    public async Task AddItemOrUpdateQuantityCommandHandler_ShouldAddItem()
    {
        // Arrange
        //register user
        var customerUserRegisterDto = new RegisterDto("additemorupdatequantity@company.com", "password", "password", nameof(Customer));
        var userId = await _testDatabaseFixture.RegisterTestUserAsync(customerUserRegisterDto);
        CustomerId customerId;
        CartId cartId;
        Guid productId;
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            //create customer
            customerId = await TestDatabaseFixture.CreateTestCustomerAsync(userId, context);
            //create cart
            cartId = await TestDatabaseFixture.CreateTestCartAsync(customerId, context);
            //create product
            productId = await TestDatabaseFixture.CreateTestProductAsync(context);
        }
        //create cart item
        var cartItem = CartItem.Create(ProductId.From(productId), "abc product", "best product", 1m);


        // Act
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var sut = new AddItemOrUpdateQuantityCommand.Handler(context);
            var result = await sut.Handle(new AddItemOrUpdateQuantityCommand(cartId, cartItem), CancellationToken.None);
        }
        // Assert
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var handler = new GetCartQuery.Handler(context);
            var result = await handler.Handle(new GetCartQuery(cartId), CancellationToken.None);
            var cart = result.Value;
            cart.Should().NotBeNull();
            cart.Items.Should().NotBeEmpty();
            cart.Items.Should().HaveCount(1);
            cart.Items.Should().ContainEquivalentOf(cartItem);
        }
    }

    [Fact]
    public async Task AddItemOrUpdateQuantityCommandHandler_ShouldUpdateQuantity()
    {
        // Arrange
        //register user
        var customerUserRegisterDto = new RegisterDto("updatequantity@gmail.com", "password", "password", nameof(Customer));
        Guid userId = await _testDatabaseFixture.RegisterTestUserAsync(customerUserRegisterDto);

        CustomerId customerId;
        CartId cartId;
        CartItem cartItem;
        Guid productId;
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            //create customer
            customerId = await TestDatabaseFixture.CreateTestCustomerAsync(userId, context);
            //create cart
            cartId = await TestDatabaseFixture.CreateTestCartAsync(customerId, context);
            //create product
            productId = await TestDatabaseFixture.CreateTestProductAsync(context);
            //create cart item
            cartItem = CartItem.Create(ProductId.From(productId), "abc product", "best product", 1m);
            //add cart item
            var sut = new AddItemOrUpdateQuantityCommand.Handler(context);
            var result = await sut.Handle(new AddItemOrUpdateQuantityCommand(cartId, cartItem), CancellationToken.None);
        }

        // Act
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var sut = new AddItemOrUpdateQuantityCommand.Handler(context);
            var result = await sut.Handle(new AddItemOrUpdateQuantityCommand(cartId, cartItem), CancellationToken.None);
        }

        // Assert
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var handler = new GetCartQuery.Handler(context);
            var result = await handler.Handle(new GetCartQuery(cartId), CancellationToken.None);
            var cart = result.Value;
            cart.Should().NotBeNull();
            cart.Items.Should().NotBeEmpty();
            cart.Items.Should().HaveCount(1);
            cart.Items.Should().ContainEquivalentOf(cartItem, options => options.Excluding(item => item.Quantity));
            cart.Items.Should().ContainEquivalentOf(cartItem, options => options.Excluding(item => item.Quantity));
        }
    }
}
