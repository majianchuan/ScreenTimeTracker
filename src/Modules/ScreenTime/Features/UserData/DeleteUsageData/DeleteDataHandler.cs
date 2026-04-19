using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserData.DeleteUsageData;

public class DeleteUsageDataHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<DeleteUsageDataCommand>
{
    public async ValueTask<Unit> Handle(DeleteUsageDataCommand request, CancellationToken cancellationToken)
    {
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue);
        var endTime = request.EndDate.ToDateTime(TimeOnly.MinValue).AddDays(1);

        await context.AppUsageSessions
            .Where(x => startTime <= x.StartTime && x.EndTime < endTime)
            .ExecuteDeleteAsync(cancellationToken);

        return Unit.Value;
    }
}