using FastEndpoints;

namespace ScreenTimeTracker.Hosts.Desktop;

public class DesktopGroup : Group
{
    public DesktopGroup()
    {
        Configure("desktop", ep =>
        {
        });
    }
}
