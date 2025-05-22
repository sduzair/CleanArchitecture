using Application.Carts.Commands;
using Application.Customers.Commands;
using Application.IntegrationTests;

using Domain.Customers.ValueObjects;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Persistence.Identity.Roles;

using Presentation.Authentication;

using WebApp.IntegrationTests.Fixtures;

namespace WebApp.IntegrationTests.Application.Carts.Commands;

[Collection(nameof(TestDatabaseCollection))]
public class CreateCartCommandHandlerTest
{
    private readonly TestDatabaseFixture _testDatabaseFixture;

    public CreateCartCommandHandlerTest(TestDatabaseFixture testDatabaseFixture) => _testDatabaseFixture = testDatabaseFixture;

    [Fact]
    public async Task CreateCartCommandHandler_ShouldCreateCart()
    {
        // Arrange
        /// register user with <see cref="Customer"/> role
        var customerUserRegisterDto = new RegisterDto("createcart@company.com", "password", "password", nameof(Customer));
        var userId = await _testDatabaseFixture.RegisterTestUserAsync(customerUserRegisterDto);
        CustomerId customerId;
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            // create customer
            customerId = await TestDatabaseFixture.CreateTestCustomerAsync(userId, context);
        }

        // Act
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var sut = new CreateCartCommand.Handler(context);
            var result = await sut.Handle(new CreateCartCommand(customerId), CancellationToken.None);
        }

        // Assert
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var cart = await context.Carts
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            cart.Should().NotBeNull();
            cart!.CustomerId.Should().Be(customerId);
        }
    }
}
