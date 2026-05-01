using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Tracking.TrackActiveSession;

public record SaveActiveSessionCommand() : IRequest;


public class SaveActiveSessionHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider) : IRequestHandler<SaveActiveSessionCommand>
{
    public async ValueTask<Unit> Handle(SaveActiveSessionCommand request, CancellationToken cancellationToken)
    {
        if (activeSessionStore.Current is null) return Unit.Value;

        var now = timeProvider.GetLocalNow().DateTime;

        var existing = await context.AppUsageSessions
            .FirstOrDefaultAsync(x =>
                x.AppId == activeSessionStore.Current.AppId &&
                x.StartTime == activeSessionStore.Current.StartTime,
                cancellationToken);

        if (existing is null)
        {
            var sessionEntity = AppUsageSession.Create(
                appId: activeSessionStore.Current.AppId,
                startTime: activeSessionStore.Current.StartTime,
                endTime: now
            );

            context.AppUsageSessions.Add(sessionEntity);
        }
        else
            existing.UpdateEndTime(now);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
