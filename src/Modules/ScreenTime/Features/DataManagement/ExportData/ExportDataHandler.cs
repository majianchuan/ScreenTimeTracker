using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ExportData;

public class ExportDataHandler(
    ILogger<ExportDataHandler> logger,
    ScreenTimeDbContext context)
    : IRequestHandler<ExportDataQuery, ExportDataResponse>
{
    public async ValueTask<ExportDataResponse> Handle(ExportDataQuery request, CancellationToken cancellationToken)
    {
        var appCategories = await context.AppCategories
            .Select(x => new { x.Name, x.Color, x.IconPath })
            .ToListAsync(cancellationToken);

        ExportDataResponse.AppCategory[] appCategoryResults;
        using (var semaphore = new SemaphoreSlim(10))
        {
            appCategoryResults = await Task.WhenAll(appCategories
               .Select(async x =>
               {
                   await semaphore.WaitAsync(cancellationToken);
                   try
                   {
                       return new ExportDataResponse.AppCategory(
                           x.Name,
                           x.Color,
                           await GetIconAsync(x.IconPath, cancellationToken));
                   }
                   finally
                   {
                       semaphore.Release();
                   }
               })
               );
        }

        var apps = await context.Apps
            .Select(x => new { x.Name, x.Color, x.ProcessName, x.IsAutoUpdateEnabled, CategoryName = x.AppCategory!.Name, x.IconPath })
            .ToListAsync(cancellationToken);

        ExportDataResponse.App[] appResults;
        using (var semaphore = new SemaphoreSlim(10))
        {
            appResults = await Task.WhenAll(apps
               .Select(async x =>
               {
                   await semaphore.WaitAsync(cancellationToken);
                   try
                   {
                       return new ExportDataResponse.App(
                       x.Name,
                       x.Color,
                       x.ProcessName,
                       x.IsAutoUpdateEnabled,
                       x.CategoryName,
                       await GetIconAsync(x.IconPath, cancellationToken));
                   }
                   finally
                   {
                       semaphore.Release();
                   }
               }));
        }

        var appUsageSessions = await context.AppUsageSessions
            .Select(x => new ExportDataResponse.AppUsageSession(
                x.App!.ProcessName,
                x.StartTime.ToString("O"),
                x.EndTime.ToString("O")
                ))
            .ToListAsync(cancellationToken);

        return new ExportDataResponse([.. appCategoryResults], [.. appResults], [.. appUsageSessions]);
    }

    public async Task<ExportDataResponse.Icon?> GetIconAsync(string? iconPath, CancellationToken cancellationToken)
    {
        if (iconPath is null || (!File.Exists(iconPath)))
            return null;

        try
        {
            string extension = Path.GetExtension(iconPath);
            byte[] fileBytes = await File.ReadAllBytesAsync(iconPath, cancellationToken);
            return new ExportDataResponse.Icon(extension, fileBytes);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Failed to read icon file: {iconPath}", iconPath);
            return null;
        }
    }
}