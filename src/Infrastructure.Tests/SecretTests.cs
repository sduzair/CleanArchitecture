using System.ComponentModel;
using System.Text.Json;

using Infrastructure.Utilities;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Tests;

public sealed class SecretTests
{
    [Fact]
    public void RevealToString()
    {
        var secret = new Secret("foo");
        Assert.Equal("foo", secret.Reveal());
        Assert.NotEqual("foo", secret.ToString());
    }

    [Fact]
    public void SystemTestJsonDoesNotRevealValue()
    {
        var secret = new Secret("foo");
        string json = JsonSerializer.Serialize(secret);
        Assert.Equal("{}", json);
    }

    [Fact]
    public void SystemTestJsonDoesNotRevealValue_Field()
    {
        var secret = new Secret("foo");
        var options = new JsonSerializerOptions { IncludeFields = true };
        string json = JsonSerializer.Serialize(secret, options);
        Assert.Equal("{}", json);
    }

    [Fact]
    public void NewtonsoftJsonDoesNotRevealValue()
    {
        var secret = new Secret("foo");
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(secret);
        Assert.Equal("{}", json);
    }

    [Fact]
    public void CanConvertFromString()
    {
        object? v = TypeDescriptor.GetConverter(typeof(Secret)).ConvertFromString("foo");
        // if v is null 
        Assert.NotNull(v);
        Secret secret = (Secret)v;
        Assert.Equal("foo", actual: secret.Reveal());
    }

    [Fact]
    public void TypeConverterToStringDoesNotRevealValue()
    {
        var secret = new Secret("foo");
        Assert.Throws<InvalidOperationException>(() => TypeDescriptor.GetConverter(typeof(Secret)).ConvertToString(secret));
    }

    [Fact]
    public async Task CanBindConfiguration()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseSetting("Password", "Pa$$w0rd");
        builder.Services.Configure<SampleOptions>(builder.Configuration);
        await using var app = builder.Build();
        var configuration = app.Services.GetRequiredService<IOptions<SampleOptions>>().Value;

        Assert.Equal("Pa$$w0rd", configuration.Password?.Reveal());
    }

    private sealed class SampleOptions
    {
        public Secret? Password { get; set; }
    }
}