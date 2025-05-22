using Application.IntegrationTests;
using Application.Products.Commands;

using Domain.Products;
using Domain.Products.ValueObjects;

using FluentAssertions;

using WebApp.IntegrationTests.Fixtures;

namespace WebApp.IntegrationTests.Application.Products;

[Collection(nameof(TestDatabaseCollection))]
public class UpdateProductCommandHandlerTest
{
    [Fact]
    public async Task UpdateProductCommandHandler_ShouldUpdateProduct()
    {
        // Arrange 
        var productId = ProductId.Create();
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var product = Product.Create(productId, "Test", "Test", 1, 1).Value;
            context.Products.Add(product);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var sut = new UpdateProductCommand.Handler(context);
            _ = await sut.Handle(new UpdateProductCommand(productId, "Test2", "Test2", 2), CancellationToken.None);
        }

        // Assert
        using (var context = TestDatabaseFixture.CreateTestAppDbContext())
        {
            var product = await context.Products.FindAsync(new object?[] { productId }, CancellationToken.None);
            product.Should().NotBeNull();
            product!.Name.Should().Be("Test2");
            product.Description.Should().Be("Test2");
            product.Price.Should().Be(2);
        }
    }
}
