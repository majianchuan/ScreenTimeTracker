using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace ScreenTimeTracker.Modules.AppBehavior.Features;

public class AppBehaviorGroup : Group
{
    public AppBehaviorGroup()
    {
        Configure("app-behavior", ep =>
        {
            ep.Description(x => x
                .WithTags("AppBehavior"));
        });
    }
}
