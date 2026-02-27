using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace ScreenTimeTracker.Modules.Shell.Features;

public class ShellGroup : Group
{
    public ShellGroup()
    {
        Configure("shell", ep =>
        {
            ep.Description(x => x
                .WithTags("shell"));
        });
    }
}
