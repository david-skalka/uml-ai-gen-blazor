using Microsoft.EntityFrameworkCore;
using TodoAppApi;
using TodoAppApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseStaticWebAssets();

builder.Services.AddProblemDetails();
builder.Services.AddControllers()
    .AddApplicationPart(typeof(TodoListsController).Assembly);

if (!builder.Environment.IsEnvironment("Integration"))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("TodoAppApiDb")));
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.Value is "/appsettings.json" or "/appsettings.Development.json" or "/appsettings.Production.json")
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsync("""{"ApiBaseUrl":""}""");
        return;
    }

    await next();
});

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.MapControllers();
app.MapFallbackToFile("index.html");

if (!app.Environment.IsEnvironment("Integration"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
}

app.Run();

// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program;
