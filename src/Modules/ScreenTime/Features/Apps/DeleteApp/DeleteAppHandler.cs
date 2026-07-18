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
        App? app = await context.Apps.FindAsync([request.AppId], cancellationToken);
        if (app is null)
            return Unit.Value;

        if (app.IsSystem)
            throw new InvalidOperationException("Cannot delete a system app.");

        // 把所有 App 的数据都删除
        await context.AppUsageSessions
            .Where(log => log.AppId == request.AppId)
            .ExecuteDeleteAsync(cancellationToken);

        context.Apps.Remove(app);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}