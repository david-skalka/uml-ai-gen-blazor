namespace TodoAppTest.E2e.Infrastructure;

public static class E2ETestHost
{
    private static E2EWebApplicationFactory<TodoApp.TestHarness.TestHarnessEntryPoint>? _factory;
    private static readonly SemaphoreSlim StartLock = new(1, 1);

    public static string BaseUrl =>
        (_factory?.BaseUrl ?? throw new InvalidOperationException("E2E host has not been started.")).TrimEnd('/');

    public static IServiceProvider ApiServices => _factory!.KestrelServices;

    public static async Task EnsureStartedAsync()
    {
        if (_factory is not null)
            return;

        await StartLock.WaitAsync();
        try
        {
            if (_factory is not null)
                return;

            _factory = new E2EWebApplicationFactory<TodoApp.TestHarness.TestHarnessEntryPoint>();
            _ = _factory.CreateClient();
        }
        finally
        {
            StartLock.Release();
        }
    }

    public static async Task DisposeAsync()
    {
        if (_factory is null)
            return;

        await _factory.DisposeAsync();
        _factory = null;
    }
}
