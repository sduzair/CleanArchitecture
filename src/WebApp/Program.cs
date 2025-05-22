using Application;

using Infrastructure;

using Presentation;
using Persistence;

//This is the entry point of the application and acts as the Composition Root
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddPersistence(builder.Environment);
    builder.Services.AddInfrastructure(builder.Environment);
    builder.Services.AddApplication(builder.Environment);
    builder.Services.AddPresentation();
}

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseExceptionHandler();  //for unhandled exceptions in production
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    //app.ApplyMigrations();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UsePresentation();

app.MapControllers();

app.Run();

public partial class Program { }