using Microsoft.Playwright;

namespace TodoAppTest.E2e.Utils;

public static class PlaywrightPageExtensions
{
    public static ILocator MainTableRows(this IPage page) =>
        page.Locator("table.mud-table-root").First.Locator("tbody tr");

    public static ILocator ActiveDialog(this IPage page) =>
        page.GetByRole(AriaRole.Dialog);

    public static async Task GotoAppPageAsync(this IPage page, string baseUrl, string path, int expectedRowCount)
    {
        await page.GotoAsync($"{baseUrl}{path}", new()
        {
            WaitUntil = WaitUntilState.NetworkIdle,
            Timeout = 60_000
        });

        await page.MainTableRows().EventuallyHaveCountAsync(expectedRowCount);
    }

    public static async Task EventuallyHaveCountAsync(this ILocator locator, int count)
    {
        await EventuallyAssertions.EventuallyAsync(async () =>
        {
            (await locator.CountAsync()).Should().Be(count);
        });
    }

    public static ILocator TableRowWithCell(this IPage page, string cellText) =>
        page.GetByRole(AriaRole.Row)
            .Filter(new() { Has = page.GetByRole(AriaRole.Cell, new() { Name = cellText, Exact = true }) });
}
