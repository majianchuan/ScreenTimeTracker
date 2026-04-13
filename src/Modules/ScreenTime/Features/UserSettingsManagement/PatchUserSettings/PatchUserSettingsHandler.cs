using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserPreferencesManagement.PatchUserSettings;

public class PatchUserSettingsHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<PatchUserSettingsCommand>
{
    public async ValueTask<Unit> Handle(PatchUserSettingsCommand request, CancellationToken cancellationToken)
    {
        UserSettings userSettings = await context.UserSettings.SingleAsync(cancellationToken);

        if (request.SamplingInterval is not null)
            userSettings.UpdateSamplingInterval(request.SamplingInterval.Value);
        if (request.IdleDetection is not null)
            userSettings.UpdateIdleDetection(request.IdleDetection.Value);
        if (request.IdleTimeout is not null)
            userSettings.UpdateIdleTimeout(request.IdleTimeout.Value);
        if (request.AppInfoStaleThreshold is not null)
            userSettings.UpdateAppInfoStaleThreshold(request.AppInfoStaleThreshold.Value);
        if (request.AggregationInterval is not null)
            userSettings.UpdateAggregationInterval(request.AggregationInterval.Value);
        if (request.AppIconDirectory is not null)
            userSettings.UpdateAppIconDirectory(request.AppIconDirectory);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}