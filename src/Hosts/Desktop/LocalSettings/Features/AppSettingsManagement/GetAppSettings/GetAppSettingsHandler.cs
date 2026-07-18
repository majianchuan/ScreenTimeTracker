using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.Persistence;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.GetAppSettings;

public class GetAppSettingsHandler(
    LocalSettingsDbContext context
    ) : IRequestHandler<GetAppSettingsQuery, GetAppSettingsResult>
{
    public async ValueTask<GetAppSettingsResult> Handle(GetAppSettingsQuery request, CancellationToken cancellationToken)
    {
        AppSettings desktopPreferences = await context.AppSettings.AsNoTracking().SingleAsync(cancellationToken);

        return new GetAppSettingsResult(
            desktopPreferences.DefaultUIOpenMode,
            desktopPreferences.IsAutoStartEnabled,
            desktopPreferences.IsSilentStartEnabled,
            desktopPreferences.Language
        );
    }
}