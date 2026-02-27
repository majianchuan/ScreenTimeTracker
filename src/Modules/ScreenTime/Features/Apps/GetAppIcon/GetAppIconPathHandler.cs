using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppIcon;

public class GetAppIconPathHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetAppIconPathQuery, string?>
{
    public async ValueTask<string?> Handle(GetAppIconPathQuery request, CancellationToken cancellationToken)
    {
        var app = await context.Apps
            .AsNoTracking()
            .FirstOrDefaultAsync(
                app => app.Id == request.Id,
                cancellationToken
            );
        return app?.IconPath;
    }
}