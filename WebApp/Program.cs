using Application;

using Infrastructure;

using Presentation;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        {
            builder.Services.AddPresentation()
                .AddInfrastructure(builder.Configuration)
                .AddApplication()
                .AddPresentation();
        }

        var app = builder.Build();

        app.UsePresentation(app.Environment);
        app.Run();
    }
}