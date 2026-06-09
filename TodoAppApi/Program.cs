using Microsoft.EntityFrameworkCore;
using TodoAppApi.Migrations;

namespace TodoAppApi;

public class Program
{
    public static void Main(string[] args)
    {
        _ = typeof(ApplicationDbContextModelSnapshot);
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddProblemDetails();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(api =>
                $"{api.ActionDescriptor.RouteValues["controller"]}{api.ActionDescriptor.RouteValues["action"]}");
        });

        if (!builder.Environment.IsEnvironment("Integration"))
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("TodoAppApiDb")));

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                if (builder.Environment.IsEnvironment("Integration"))
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
                else
                {
                    policy.WithOrigins(
                            "http://localhost:5154",
                            "https://localhost:7286",
                            "http://localhost:5173",
                            "https://localhost:7173",
                            "http://127.0.0.1:18766")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();
        app.UseCors();
        app.MapControllers();

        if (!app.Environment.IsEnvironment("Integration"))
        {
            using var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
        }

        app.Run();
    }
}