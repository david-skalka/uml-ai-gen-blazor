using System.Diagnostics;
using System.Text.Json;
using TodoAppApi;

namespace TodoAppTest.E2e.Infrastructure;

public static class E2ETestHost
{
    public const string ApiBaseUrl = "http://127.0.0.1:18765";
    public const string BlazorBaseUrl = "http://127.0.0.1:18766";

    private static E2EWebApplicationFactory<Program>? _apiFactory;
    private static Process? _blazorProcess;
    private static string? _appsettingsBackup;
    private static string? _appsettingsPath;
    private static string? _devAppsettingsBackup;
    private static string? _devAppsettingsPath;
    private static readonly SemaphoreSlim StartLock = new(1, 1);
    private static readonly HttpClient WaitClient = new() { Timeout = TimeSpan.FromSeconds(2) };

    public static IServiceProvider ApiServices =>
        (_apiFactory ?? throw new InvalidOperationException("E2E host has not been started.")).KestrelServices;

    public static async Task EnsureStartedAsync()
    {
        if (_apiFactory is not null)
            return;

        await StartLock.WaitAsync();
        try
        {
            if (_apiFactory is not null)
                return;

            _apiFactory = new E2EWebApplicationFactory<Program>();
            _ = _apiFactory.CreateClient();

            await StartBlazorDevServerAsync();
        }
        finally
        {
            StartLock.Release();
        }
    }

    private static async Task StartBlazorDevServerAsync()
    {
        var solutionRoot = FindSolutionRoot();
        var blazorProject = Path.Combine(solutionRoot, "TodoApp.Blazor", "TodoApp.Blazor.csproj");
        _appsettingsPath = Path.Combine(solutionRoot, "TodoApp.Blazor", "wwwroot", "appsettings.json");
        _devAppsettingsPath = Path.Combine(solutionRoot, "TodoApp.Blazor", "wwwroot", "appsettings.Development.json");
        _appsettingsBackup = await File.ReadAllTextAsync(_appsettingsPath);
        _devAppsettingsBackup = await File.ReadAllTextAsync(_devAppsettingsPath);

        var appsettingsJson = JsonSerializer.Serialize(new { ApiBaseUrl = ApiBaseUrl.TrimEnd('/') });
        await File.WriteAllTextAsync(_appsettingsPath, appsettingsJson);
        await File.WriteAllTextAsync(_devAppsettingsPath, appsettingsJson);

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"run --project \"{blazorProject}\" --urls {BlazorBaseUrl} --no-launch-profile",
            WorkingDirectory = solutionRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _blazorProcess = Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start Blazor dev server for E2E tests.");

        await WaitForUrlAsync(BlazorBaseUrl, TimeSpan.FromSeconds(120));
    }

    public static async Task DisposeAsync()
    {
        if (_blazorProcess is not null)
        {
            try
            {
                if (!_blazorProcess.HasExited)
                    _blazorProcess.Kill(entireProcessTree: true);
            }
            catch
            {
                // ignored
            }

            _blazorProcess.Dispose();
            _blazorProcess = null;
        }

        if (_appsettingsPath is not null && _appsettingsBackup is not null)
        {
            await File.WriteAllTextAsync(_appsettingsPath, _appsettingsBackup);
            _appsettingsPath = null;
            _appsettingsBackup = null;
        }

        if (_devAppsettingsPath is not null && _devAppsettingsBackup is not null)
        {
            await File.WriteAllTextAsync(_devAppsettingsPath, _devAppsettingsBackup);
            _devAppsettingsPath = null;
            _devAppsettingsBackup = null;
        }

        if (_apiFactory is not null)
        {
            await _apiFactory.DisposeAsync();
            _apiFactory = null;
        }
    }

    private static string FindSolutionRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir.FullName, "todoapp.slnx")))
                return dir.FullName;

            dir = dir.Parent;
        }

        throw new InvalidOperationException("Could not locate solution root (todoapp.slnx).");
    }

    private static async Task WaitForUrlAsync(string url, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = await WaitClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    return;
            }
            catch
            {
                // server still starting
            }

            await Task.Delay(500);
        }

        throw new TimeoutException($"Timed out waiting for {url}.");
    }
}
