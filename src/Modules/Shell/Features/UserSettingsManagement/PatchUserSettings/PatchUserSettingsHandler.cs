using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.Shell.Domain;
using ScreenTimeTracker.Modules.Shell.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.PatchUserSettings;

public class PatchUserSettingsHandler(
    ShellDbContext context,
    IStartupManager windowsStartupManager
    ) : IRequestHandler<PatchUserSettingsCommand>
{
    public async ValueTask<Unit> Handle(PatchUserSettingsCommand request, CancellationToken cancellationToken)
    {
        UserSettings userSettings = await context.UserSettings.SingleAsync(cancellationToken);

        if (request.UIOpenMode is not null)
            userSettings.UpdateUIOpenMode(request.UIOpenMode);
        if (request.AutoStart is not null)
        {
            if (request.AutoStart.Value && !windowsStartupManager.IsEnabled())
                windowsStartupManager.Enable();
            else if (!request.AutoStart.Value && windowsStartupManager.IsEnabled())
                windowsStartupManager.Disable();
            userSettings.UpdateAutoStart(request.AutoStart.Value);
        }
        if (request.SilentStart is not null)
            userSettings.UpdateSilentStart(request.SilentStart.Value);
        if (request.Language is not null)
            userSettings.UpdateLanguage(request.Language);
        if (request.WindowDestroyOnClose is not null)
            userSettings.UpdateWindowDestroyOnClose(request.WindowDestroyOnClose.Value);

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