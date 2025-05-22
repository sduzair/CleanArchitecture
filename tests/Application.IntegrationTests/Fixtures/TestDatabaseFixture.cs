using System.Net.Http.Json;

using Application.Carts.Commands;
using Application.Customers.Commands;
using Application.Products.Commands;

using Domain.Carts.ValueObjects;
using Domain.Customers.ValueObjects;

using Microsoft.EntityFrameworkCore;

using Persistence;
using Persistence.Identity;
using Persistence.Identity.Roles;

using Presentation.Authentication;

using WebApp.IntegrationTests.Fixtures;

namespace Application.IntegrationTests;

public sealed class TestDatabaseFixture : IAsyncLifetime
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public TestDatabaseFixture()
    {
        using var context = CreateTestAppDbContext();
        context.Database.EnsureDeletedAsync();
        context.Database.EnsureCreatedAsync();
        _factory = new TestWebApplicationFactory<Program>();
    }

    public static AppDbContext CreateTestAppDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .EnableSensitiveDataLogging()
            .UseSqlite($"Data Source={Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "cleanarchitecture_test.db")}");

        return new AppDbContext(optionsBuilder.Options);
    }
    public async Task InitializeAsync()
    {
        RegisterDto testCustomerUserRegisterDto = new("testuser@company.com", "password", "password", nameof(Customer));
        var userId = await RegisterTestUserAsync(testCustomerUserRegisterDto);

        using var context = CreateTestAppDbContext();
        var customerId = await CreateTestCustomerAsync(userId, context);
        await CreateTestCartAsync(customerId, context);
    }

    public static async Task<CartId> CreateTestCartAsync(CustomerId customerId, AppDbContext context)
    {
        var command = new CreateCartCommand(customerId);
        var handler = new CreateCartCommand.Handler(context);
        var result = await handler.Handle(command, CancellationToken.None);
        return result.Value;
    }

    public static async Task<CustomerId> CreateTestCustomerAsync(Guid userId, AppDbContext context)
    {
        var command = new CreateCustomerCommand(userId);
        var handler = new CreateCustomerCommand.Handler(context);
        var result = await handler.Handle(command, CancellationToken.None);
        return result.Value;
    }

    internal static async Task<Guid> CreateTestProductAsync(AppDbContext context)
    {
        var command = new CreateProductCommand("Test Product", "Best Product", 3.99m, 34);
        var handler = new CreateProductCommand.Handler(context);
        var result = await handler.Handle(command, CancellationToken.None);
        return result.Value;
    }

    public async Task<Guid> RegisterTestUserAsync(RegisterDto registerDto)
    {
        var client = _factory.CreateClient();

        /// corresponds to the following API endpoint: <see cref="AuthController.Register"/>
        var response = await client.PostAsJsonAsync("/api/Auth/Register", registerDto);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        return Guid.Parse(content!["userId"].ToString()!);
    }

    public async Task DisposeAsync()
    {
        _factory.Dispose();
        await Task.CompletedTask;
    }

}