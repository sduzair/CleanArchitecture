using Application;

using Infrastructure;

using Presentation;

using WebApp;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfrastructure(builder.Configuration)
                .AddApplication()
                .AddPresentation();
            builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
            builder.Services.AddProblemDetails();
        }

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseExceptionHandler("/Error");
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}