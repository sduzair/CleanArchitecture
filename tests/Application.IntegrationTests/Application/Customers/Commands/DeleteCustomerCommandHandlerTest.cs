using Application.Customers.Commands;
using Application.IntegrationTests;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using Presentation.Authentication;

using WebApp.IntegrationTests.Fixtures;

namespace WebApp.IntegrationTests.Application.Customers.Commands;

[Collection(nameof(TestDatabaseCollection))]
public class DeleteCustomerCommandHandlerTest
{
    private readonly TestDatabaseFixture _testDatabaseFixture;

    public DeleteCustomerCommandHandlerTest(TestDatabaseFixture testDatabaseFixture) => _testDatabaseFixture = testDatabaseFixture;

    [Fact]
    public async Task DeleteCustomerCommandHandler_ShouldDeleteCustomer()
    {
        // Arrange
        RegisterDto customerUserRegisterDto = new("testuser2@company.com", "password", "password", "Customer");
        var userId = await _testDatabaseFixture.RegisterTestUserAsync(customerUserRegisterDto);

        // Act
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var sut = new DeleteCustomerCommand.Handler(context);
            await sut.Handle(new DeleteCustomerCommand(userId), CancellationToken.None);
        }
        // Assert
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var customer = await context.Customers
                .Where(c => c.ApplicationUserId == userId)
                .SingleOrDefaultAsync(CancellationToken.None);

            customer.Should().BeNull();
        }
    }
}
