using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace ScreenTimeTracker.Hosts.Desktop;

public class DesktopGroup : Group
{
    public DesktopGroup()
    {
        Configure("desktop", ep =>
        {
            ep.Description(x => x
                .WithTags("desktop"));
        });
    }
}
