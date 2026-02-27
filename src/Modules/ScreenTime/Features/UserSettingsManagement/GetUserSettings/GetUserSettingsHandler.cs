using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.GetUserSettings;

public class GetUserSettingsHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetUserSettingsQuery, GetUserSettingsResult>
{
    public async ValueTask<GetUserSettingsResult> Handle(GetUserSettingsQuery request, CancellationToken cancellationToken)
    {
        UserSettings userSettings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);

        return new GetUserSettingsResult(
            SamplingInterval: userSettings.SamplingInterval,
            IdleDetection: userSettings.IdleDetection,
            IdleTimeout: userSettings.IdleTimeout,
            AppInfoStaleThreshold: userSettings.AppInfoStaleThreshold,
            AggregationInterval: userSettings.AggregationInterval,
            AppIconDirectory: userSettings.AppIconDirectory
        );
    }
}