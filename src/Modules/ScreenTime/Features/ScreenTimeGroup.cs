using FastEndpoints;

namespace ScreenTimeTracker.Modules.ScreenTime.Features;

public class ScreenTimeGroup : Group
{
    public ScreenTimeGroup()
    {
        Configure("screen-time", ep =>
        {
        });
    }
}
