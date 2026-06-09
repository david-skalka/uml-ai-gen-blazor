using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TodoApp.Api;
using TodoApp.Blazor;
using TodoApp.Blazor.Api;
using TodoApp.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5000";

builder.Services.AddMudServices();
builder.Services.AddScoped<IErrorHandlerService, ErrorHandlerService>();

builder.Services.AddScoped(_ => new HttpClient(new JHipsterResponseHandler
    {
        InnerHandler = new HttpClientHandler()
    })
    {
        BaseAddress = new Uri(apiBaseUrl.TrimEnd('/') + "/")
    });
builder.Services.AddScoped<IClient>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new Client(apiBaseUrl, httpClient)
    {
        ReadResponseAsString = true
    };
});

await builder.Build().RunAsync();
