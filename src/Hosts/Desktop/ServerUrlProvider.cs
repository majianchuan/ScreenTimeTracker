using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace ScreenTimeTracker.Hosts.Desktop;

public interface IServerUrlProvider
{
    string GetServerUrl();
}

public class ServerUrlProvider(IServer server) : IServerUrlProvider
{
    private readonly IServer _server = server;

    public string GetServerUrl()
    {
        var url = _server.Features.Get<IServerAddressesFeature>()?.Addresses?.FirstOrDefault();
        if (string.IsNullOrEmpty(url))
            throw new InvalidOperationException("Server has not started or URL is not available.");

        return url;
    }
}