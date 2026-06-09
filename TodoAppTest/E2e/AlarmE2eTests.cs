using Microsoft.Playwright;
using TodoAppTest.E2e.Utils;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

public class AlarmE2ETests : E2ETestBase
{
    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Show()
    {
        await Page.GotoAppPageAsync(BaseUrl, "/alarms", DefaultSeeder.Alarms.Length);
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Create()
    {
        await Page.GotoAppPageAsync(BaseUrl, "/alarms", DefaultSeeder.Alarms.Length);
        await Page.GetByRole(AriaRole.Button, new() { Name = "New" }).ClickAsync();

        var dialog = Page.ActiveDialog();
        await dialog.GetByLabel("Title").FillAsync("Morning run");
        await dialog.GetByLabel("Time").FillAsync("2026-05-24 07:30");
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Page.MainTableRows().EventuallyHaveCountAsync(DefaultSeeder.Alarms.Length + 1);
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Edit()
    {
        const string title = "Updated";
        const string time = "2026-05-23 09:00";
        var original = DefaultSeeder.Alarms[0];

        await Page.GotoAppPageAsync(BaseUrl, "/alarms", DefaultSeeder.Alarms.Length);

        await Page.TableRowWithCell(original.Title)
            .GetByRole(AriaRole.Button, new() { Name = "Edit" })
            .ClickAsync();

        var dialog = Page.ActiveDialog();
        await dialog.GetByLabel("Title").FillAsync(title);
        await dialog.GetByLabel("Time").FillAsync(time);
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Page.MainTableRows().EventuallyHaveCountAsync(DefaultSeeder.Alarms.Length);
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Delete()
    {
        var original = DefaultSeeder.Alarms[0];

        await Page.GotoAppPageAsync(BaseUrl, "/alarms", DefaultSeeder.Alarms.Length);

        await Page.TableRowWithCell(original.Title)
            .GetByRole(AriaRole.Button, new() { Name = "Delete" })
            .ClickAsync();

        await Page.ActiveDialog()
            .GetByRole(AriaRole.Button, new() { Name = "Delete" })
            .ClickAsync();

        await Page.MainTableRows().EventuallyHaveCountAsync(DefaultSeeder.Alarms.Length - 1);
    }
}
