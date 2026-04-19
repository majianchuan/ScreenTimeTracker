using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.DeleteApp;

public class DeleteAppHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<DeleteAppCommand>
{
    public async ValueTask<Unit> Handle(DeleteAppCommand request, CancellationToken cancellationToken)
    {
        App? App = await context.Apps.FindAsync([request.Id], cancellationToken);
        if (App is null)
            return Unit.Value;

        // 把所有 App 的数据都删除
        await context.AppUsageSessions
            .Where(log => log.AppId == request.Id)
            .ExecuteDeleteAsync(cancellationToken);

        context.Apps.Remove(App);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}