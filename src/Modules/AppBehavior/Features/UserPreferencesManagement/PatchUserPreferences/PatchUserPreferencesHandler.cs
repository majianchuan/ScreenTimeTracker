using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.AppBehavior.Domain;
using ScreenTimeTracker.Modules.AppBehavior.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.PatchUserPreferences;

public class PatchUserPreferencesHandler(
    AppBehaviorDbContext context,
    IStartupManager windowsStartupManager
    ) : IRequestHandler<PatchUserPreferencesCommand>
{
    public async ValueTask<Unit> Handle(PatchUserPreferencesCommand request, CancellationToken cancellationToken)
    {
        UserPreferences userPreferences = await context.UserPreferences.SingleAsync(cancellationToken);

        if (request.UIOpenMode is not null)
            userPreferences.UpdateUIOpenMode(request.UIOpenMode);
        if (request.AutoStart is not null)
        {
            if (request.AutoStart.Value && !windowsStartupManager.IsEnabled())
                windowsStartupManager.Enable();
            else if (!request.AutoStart.Value && windowsStartupManager.IsEnabled())
                windowsStartupManager.Disable();
            userPreferences.UpdateAutoStart(request.AutoStart.Value);
        }
        if (request.SilentStart is not null)
            userPreferences.UpdateSilentStart(request.SilentStart.Value);
        if (request.Language is not null)
            userPreferences.UpdateLanguage(request.Language);
        if (request.WindowDestroyOnClose is not null)
            userPreferences.UpdateWindowDestroyOnClose(request.WindowDestroyOnClose.Value);

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