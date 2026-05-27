using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Infrastructure.Persistence;
namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.PatchAppSettings;

public class PatchAppSettingsHandler(
    LocalSettingsDbContext context,
    IStartupManager windowsStartupManager
    ) : IRequestHandler<PatchAppSettingsCommand>
{
    public async ValueTask<Unit> Handle(PatchAppSettingsCommand request, CancellationToken cancellationToken)
    {
        AppSettings appSettings = await context.AppSettings.SingleAsync(cancellationToken);

        if (request.DefaultUIOpenMode is not null)
            appSettings.UpdateUIOpenMode(request.DefaultUIOpenMode);
        if (request.IsAutoStartEnabled is not null)
        {
            if (request.IsAutoStartEnabled.Value && !windowsStartupManager.IsEnabled())
                windowsStartupManager.Enable();
            else if (!request.IsAutoStartEnabled.Value && windowsStartupManager.IsEnabled())
                windowsStartupManager.Disable();
            appSettings.UpdateAutoStart(request.IsAutoStartEnabled.Value);
        }
        if (request.IsSilentStartEnabled is not null)
            appSettings.UpdateSilentStart(request.IsSilentStartEnabled.Value);
        if (request.Language is not null)
            appSettings.UpdateLanguage(request.Language);
        if (request.ShouldDestroyWindowOnClose is not null)
            appSettings.UpdateWindowDestroyOnClose(request.ShouldDestroyWindowOnClose.Value);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

public interface IStartupManager
{
    bool IsEnabled();
    void Enable();
    void Disable();
}