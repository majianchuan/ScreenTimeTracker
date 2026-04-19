using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Tracking.TrackActiveSession;

public record SystemBecameIdleCommand(
    DateTime IdleStartedAt
) : IRequest;


public class SystemBecameIdleHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider,
    IMediator mediator) : IRequestHandler<SystemBecameIdleCommand>
{
    public async ValueTask<Unit> Handle(SystemBecameIdleCommand request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetLocalNow().DateTime;

        if (activeSessionStore.Current is not null)
            await mediator.Send(new SaveActiveSessionCommand(), cancellationToken);
        activeSessionStore.Current = new ActiveSessionState(App.IdleAppId, now);

        // 修正已有数据中空闲开始到现在范围内数据为空闲
        var affectedSessions = await context.AppUsageSessions
            .Where(s => request.IdleStartedAt <= s.EndTime)
            .ToListAsync(cancellationToken);

        foreach (var session in affectedSessions)
        {
            // 完全在空闲时间范围内
            if (request.IdleStartedAt <= session.StartTime)
                session.MarkAsIdle(App.IdleAppId);
            // 部分在空闲时间范围内
            else
            {
                var idlePartSession = AppUsageSession.Create(App.IdleAppId, request.IdleStartedAt, session.EndTime);
                context.AppUsageSessions.Add(idlePartSession);
                session.UpdateEndTime(request.IdleStartedAt);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
