using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserData.DeleteUsageData;

public class DeleteUsageDataHandler(
    ScreenTimeDbContext context,
        ILogger<DeleteUsageDataHandler> logger
    ) : IRequestHandler<DeleteUsageDataCommand>
{
    public async ValueTask<Unit> Handle(DeleteUsageDataCommand request, CancellationToken cancellationToken)
    {
        var startTime = request.StartDate.ToDateTime(TimeOnly.MinValue);
        var endTime = request.EndDate.AddDays(1).ToDateTime(TimeOnly.MinValue);

        using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await context.ActivityLogs
                .Where(x => startTime <= x.LoggedAt && x.LoggedAt < endTime)
                .ExecuteDeleteAsync(cancellationToken);
            await context.AppHourlyUsages
                .Where(x => startTime <= x.Hour && x.Hour < endTime)
                .ExecuteDeleteAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "DeleteUsageDataHandler failed");
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
        return Unit.Value;
    }
}