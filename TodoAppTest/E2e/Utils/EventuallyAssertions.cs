using FluentAssertions.Execution;

namespace TodoAppTest.E2e.Utils;

public static class EventuallyAssertions
{
    private static readonly TimeSpan DefaultWait = TimeSpan.FromSeconds(15);
    private static readonly TimeSpan DefaultPoll = TimeSpan.FromMilliseconds(200);

    public static async Task EventuallyAsync(Func<Task> check, TimeSpan? wait = null, TimeSpan? poll = null)
    {
        var deadline = DateTime.UtcNow + (wait ?? DefaultWait);
        var pollInterval = poll ?? DefaultPoll;
        Exception? last = null;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                await check();
                return;
            }
            catch (Exception ex)
            {
                last = ex;
                await Task.Delay(pollInterval);
            }
        }

        throw last ?? new TimeoutException("Eventually assertion timed out.");
    }

    public static async Task EventuallyAsync(Action check, TimeSpan? wait = null, TimeSpan? poll = null)
    {
        await EventuallyAsync(() =>
        {
            check();
            return Task.CompletedTask;
        }, wait, poll);
    }
}
