using Application.Customers.Commands;
using Application.IntegrationTests;

using Domain.Customers;
using Domain.Customers.ValueObjects;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Presentation.Authentication;

using WebApp.IntegrationTests.Fixtures;

namespace WebApp.IntegrationTests.Application.Customers.Commands;

[Collection(nameof(TestDatabaseCollection))]
public class CreateCustomerCommandHandlerTest
{
    private readonly TestDatabaseFixture _testDatabaseFixture;

    public CreateCustomerCommandHandlerTest(TestDatabaseFixture testDatabaseFixture) => _testDatabaseFixture = testDatabaseFixture;

    [Fact]
    public async Task CreateCustomerCommandHandler_ShouldCreateCustomer()
    {
        // Arrange
        RegisterDto testCustomerUserRegisterDto = new("createcustomertest@company.com", "password", "password", nameof(Customer));
        var userId = await _testDatabaseFixture.RegisterTestUserAsync(testCustomerUserRegisterDto);


        // Act
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var sut = new CreateCustomerCommand.Handler(context);
            var result = await sut.Handle(new CreateCustomerCommand(userId), CancellationToken.None);
        }
        // Assert
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var customer = await context.Customers
                .Where(c => c.ApplicationUserId == userId)
                .SingleOrDefaultAsync(CancellationToken.None);

            customer.Should().NotBeNull();
        }
    }
}
