using Microsoft.Extensions.DependencyInjection;
using TodoAppApi;
using TodoApp.TestHarness;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e.Infrastructure;

public class E2EHarnessSmokeTests
{
    [Test]
    public async Task Harness_serves_api_and_blazor_assets()
    {
        await using var factory = new E2EWebApplicationFactory<TestHarnessEntryPoint>();
        _ = factory.CreateClient();

        var db = factory.KestrelServices.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        new DefaultSeeder().Seed(db);

        using var http = factory.CreateKestrelClient();

        var index = await http.GetAsync("/");
        Assert.That(index.IsSuccessStatusCode);

        var api = await http.GetStringAsync("/api/todo-lists");
        Assert.That(api, Does.Contain("Work"));

        var indexHtml = await http.GetStringAsync("/");
        Assert.That(indexHtml, Does.Contain("blazor"));

        var devSettings = await http.GetStringAsync("/appsettings.Development.json");
        Assert.That(devSettings, Does.Contain("\"ApiBaseUrl\":").And.Not.Contain("localhost"));
    }
}
