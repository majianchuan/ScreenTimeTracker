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
            AppIconDirectory: userSettings.AppIconDirectory,
            AppInfoStaleThreshold: userSettings.AppInfoStaleThreshold,
            ActiveSessionAutoSaveInterval: userSettings.ActiveSessionAutoSaveInterval,

            IsIdleDetectionEnabled: userSettings.IsIdleDetectionEnabled,
            IdleThreshold: userSettings.IdleThreshold,
            IdleDetectionPollingInterval: userSettings.IdleDetectionPollingInterval,

            MinValidSessionDuration: userSettings.MinValidSessionDuration,
            SessionMergeTolerance: userSettings.SessionMergeTolerance,
            SessionOptimizationInterval: userSettings.SessionOptimizationInterval,

            DayCutoffHour: userSettings.DayCutoffHour
        );
    }
}