using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace ScreenTimeTracker.Modules.ScreenTime.Features;

public class ScreenTimeGroup : Group
{
    public ScreenTimeGroup()
    {
        Configure("screen-time", ep =>
        {
            ep.Description(x => x
                .WithTags("screen-time"));
        });
    }
}
