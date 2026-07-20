using FastEndpoints;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features;

public class LocalSettingsGroup : SubGroup<DesktopGroup>
{
    public LocalSettingsGroup()
    {
        Configure("local-settings", ep =>
        {
        });
    }
}
