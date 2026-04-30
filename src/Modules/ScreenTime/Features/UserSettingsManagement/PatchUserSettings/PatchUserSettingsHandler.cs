using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.PatchUserSettings;

public class PatchUserSettingsHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<PatchUserSettingsCommand>
{
    public async ValueTask<Unit> Handle(PatchUserSettingsCommand request, CancellationToken cancellationToken)
    {
        UserSettings userSettings = await context.UserSettings.SingleAsync(cancellationToken);

        if (request.AppIconDirectory is not null)
            userSettings.UpdateAppIconDirectory(request.AppIconDirectory);
        if (request.AppInfoStaleThreshold is not null)
            userSettings.UpdateAppInfoStaleThreshold(request.AppInfoStaleThreshold.Value);
        if (request.ActiveSessionAutoSaveInterval is not null)
            userSettings.UpdateActiveSessionAutoSaveInterval(request.ActiveSessionAutoSaveInterval.Value);

        if (request.IsIdleDetectionEnabled is not null)
            userSettings.UpdateIsIdleDetectionEnabled(request.IsIdleDetectionEnabled.Value);
        if (request.IdleThreshold is not null)
            userSettings.UpdateIdleThreshold(request.IdleThreshold.Value);
        if (request.IdleDetectionPollingInterval is not null)
            userSettings.UpdateIdleDetectionPollingInterval(request.IdleDetectionPollingInterval.Value);

        if (request.MinValidSessionDuration is not null)
            userSettings.UpdateMinValidSessionDuration(request.MinValidSessionDuration.Value);
        if (request.SessionMergeTolerance is not null)
            userSettings.UpdateSessionMergeTolerance(request.SessionMergeTolerance.Value);
        if (request.SessionOptimizationInterval is not null)
            userSettings.UpdateSessionOptimizationInterval(request.SessionOptimizationInterval.Value);

        if (request.DayBoundaryOffsetHours is not null)
            userSettings.UpdateDayCutoffHour(request.DayBoundaryOffsetHours.Value);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}