using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ExportData;

public class ExportDataHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<ExportDataQuery, ExportDataResponse>
{
    public async ValueTask<ExportDataResponse> Handle(ExportDataQuery request, CancellationToken cancellationToken)
    {
        var sessions = await context.AppUsageSessions
            .Select(x => new UsageSession(
                x.App!.Name,
                x.App!.ProcessName,
                x.StartTime.ToString("O"),
                x.EndTime.ToString("O")
                ))
            .ToListAsync(cancellationToken);
        return new ExportDataResponse([.. sessions]);
    }
}