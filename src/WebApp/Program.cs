using Application;

using Infrastructure;

using Presentation;

//This is the entry point of the application and acts as the Composition Root
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

{
    builder.Services.AddInfrastructure(builder.Environment);
    builder.Services.AddApplication();
    builder.Services.AddPresentation();
}

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();  //for unhandled exceptions in production
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UsePresentation();

app.MapControllers();

app.Run();