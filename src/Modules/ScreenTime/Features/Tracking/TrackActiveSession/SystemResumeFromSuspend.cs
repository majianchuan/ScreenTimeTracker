using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Tracking.TrackActiveSession;

public record SystemResumeFromSuspendCommand(
    DateTime SuspendStartedAt
) : IRequest;


public class SystemResumeFromSuspendHandler(
    ScreenTimeDbContext context,
    IActiveSessionStore activeSessionStore,
    TimeProvider timeProvider,
    IMediator mediator) : IRequestHandler<SystemResumeFromSuspendCommand>
{
    public async ValueTask<Unit> Handle(SystemResumeFromSuspendCommand request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetLocalNow().DateTime;

        if (activeSessionStore.Current is not null)
            await mediator.Send(new SaveActiveSessionCommand(), cancellationToken);
        activeSessionStore.Current = null;

        // 删除已有数据中系统挂起开始到现在范围内数据
        var affectedSessions = await context.AppUsageSessions
            .Where(s => request.SuspendStartedAt <= s.EndTime)
            .ToListAsync(cancellationToken);

        foreach (var session in affectedSessions)
        {
            // 完全在系统挂起时间范围内
            if (request.SuspendStartedAt <= session.StartTime)
                session.MarkAsIdle(App.IdleAppId);
            // 部分在系统挂起时间范围内
            else
                session.UpdateEndTime(request.SuspendStartedAt);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
