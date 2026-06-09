using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoAppTest.Integration.Infrastructure;

namespace TodoAppTest.E2e.Infrastructure;

public sealed class E2eWebApplicationFactory<TProgram> : CustomWebApplicationFactory<TProgram>
    where TProgram : class
{
    private IHost? _kestrelHost;

    public string ServerAddress { get; private set; } = string.Empty;

    public IServiceProvider KestrelServices =>
        _kestrelHost?.Services ?? throw new InvalidOperationException("Kestrel host has not been started.");

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = base.CreateHost(builder);

        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.UseKestrel();
            webHostBuilder.UseUrls(E2eTestHost.ApiBaseUrl);
        });

        _kestrelHost = builder.Build();
        _kestrelHost.Start();

        var server = _kestrelHost.Services.GetRequiredService<IServer>();
        ServerAddress = server.Features.Get<IServerAddressesFeature>()?.Addresses.FirstOrDefault()
                        ?? E2eTestHost.ApiBaseUrl;

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
