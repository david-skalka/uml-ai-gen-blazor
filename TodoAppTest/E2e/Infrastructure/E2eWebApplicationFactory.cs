using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TodoAppTest.Integration.Infrastructure;

namespace TodoAppTest.E2e.Infrastructure;

public sealed class E2EWebApplicationFactory<TProgram> : CustomWebApplicationFactory<TProgram>
    where TProgram : class
{
    private IHost? _kestrelHost;

    public IServiceProvider KestrelServices =>
        _kestrelHost?.Services ?? throw new InvalidOperationException("Kestrel host has not been started.");

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = base.CreateHost(builder);

        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.UseKestrel();
            webHostBuilder.UseUrls(E2ETestHost.ApiBaseUrl);
        });

        _kestrelHost = builder.Build();
        _kestrelHost.Start();

        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && _kestrelHost is not null)
        {
            _kestrelHost.StopAsync().GetAwaiter().GetResult();
            _kestrelHost.Dispose();
            _kestrelHost = null;
        }

        base.Dispose(disposing);
    }
}
