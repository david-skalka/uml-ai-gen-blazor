using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using TodoAppTest.Integration.Infrastructure;

namespace TodoAppTest.E2e.Infrastructure;

file static class KestrelHttp
{
    internal static readonly SocketsHttpHandler Handler = new();
}

public sealed class E2EWebApplicationFactory<TProgram> : CustomWebApplicationFactory<TProgram>
    where TProgram : class
{
    private IHost? _kestrelHost;

    public string BaseUrl { get; private set; } = "";

    public IServiceProvider KestrelServices =>
        _kestrelHost?.Services ?? throw new InvalidOperationException("Kestrel host has not been started.");

    public HttpClient CreateKestrelClient()
    {
        var client = new HttpClient(KestrelHttp.Handler, disposeHandler: false);
        client.BaseAddress = new Uri(BaseUrl + "/");
        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseStaticWebAssets();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var testHost = base.CreateHost(builder);
        BaseUrl = $"http://127.0.0.1:{GetFreeTcpPort()}";

        builder.ConfigureWebHost(webHostBuilder =>
        {
            webHostBuilder.UseKestrel();
            webHostBuilder.UseUrls(BaseUrl);
        });

        _kestrelHost = builder.Build();
        _kestrelHost.Start();

        return testHost;
    }

    private static int GetFreeTcpPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
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
