using Application.IntegrationTests;
using Application.Products.Commands;

using Domain.Products.ValueObjects;

using FluentAssertions;

using WebApp.IntegrationTests.Fixtures;

namespace WebApp.IntegrationTests.Application.Products;

[Collection(nameof(TestDatabaseCollection))]
public class CreateProductCommandHandlerTest
{
    [Fact]
    public async Task CreateProductCommandHandler_ShouldCreateProduct()
    {
        // Arrange
        // Act
        ProductId productId;
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var sut = new CreateProductCommand.Handler(context);
            var result = await sut.Handle(new CreateProductCommand("Test", "Test", 1, 1), CancellationToken.None);
            productId = ProductId.From(result.Value);
        }
        // Assert
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var product = await context.Products.FindAsync(new object?[] { productId }, CancellationToken.None);
            product.Should().NotBeNull();
        }
    }

}
