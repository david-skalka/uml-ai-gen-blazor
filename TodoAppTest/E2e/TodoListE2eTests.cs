using Microsoft.Playwright;
using TodoAppTest.E2e.Utils;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

public class TodoListE2ETests : E2ETestBase
{
    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Show()
    {
        await Page.GotoAppPageAsync(BaseUrl, "/todo-list", DefaultSeeder.TodoLists.Length);
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Create()
    {
        await Page.GotoAppPageAsync(BaseUrl, "/todo-list", DefaultSeeder.TodoLists.Length);
        await Page.GetByRole(AriaRole.Button, new() { Name = "New" }).ClickAsync();

        var dialog = Page.ActiveDialog();
        await dialog.GetByLabel("Name").FillAsync("Shopping");
        await dialog.GetByLabel("Description").FillAsync("Groceries");
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Expect(Page.MainTableRows()).ToHaveCountAsync(DefaultSeeder.TodoLists.Length + 1);
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Edit()
    {
        const string name = "Updated";
        const string description = "After";
        var original = DefaultSeeder.TodoLists[0];

        await Page.GotoAppPageAsync(BaseUrl, "/todo-list", DefaultSeeder.TodoLists.Length);

        await Page.TableRowWithCell(original.Name)
            .GetByRole(AriaRole.Button, new() { Name = "Edit" })
            .ClickAsync();

        var dialog = Page.ActiveDialog();
        await dialog.GetByLabel("Name").FillAsync(name);
        await dialog.GetByLabel("Description").FillAsync(description);
        await dialog.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await Expect(Page.MainTableRows()).ToHaveCountAsync(DefaultSeeder.TodoLists.Length);
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Delete()
    {
        var original = DefaultSeeder.TodoLists[0];

        await Page.GotoAppPageAsync(BaseUrl, "/todo-list", DefaultSeeder.TodoLists.Length);

        await Page.TableRowWithCell(original.Name)
            .GetByRole(AriaRole.Button, new() { Name = "Delete" })
            .ClickAsync();

        await Page.ActiveDialog()
            .GetByRole(AriaRole.Button, new() { Name = "Delete" })
            .ClickAsync();

        await Expect(Page.MainTableRows()).ToHaveCountAsync(DefaultSeeder.TodoLists.Length - 1);
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task GroupByName()
    {
        await Page.GotoAppPageAsync(BaseUrl, "/todo-list", DefaultSeeder.TodoLists.Length);
        await Page.GetByRole(AriaRole.Tab, new() { Name = "Extra actions" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Run" }).ClickAsync();

        await Expect(Page.MainTableRows()).ToHaveCountAsync(DefaultSeeder.TodoLists.Length);
    }
}
