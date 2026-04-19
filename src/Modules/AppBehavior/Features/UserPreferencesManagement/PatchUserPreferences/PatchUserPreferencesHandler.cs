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

        if (request.DefaultUIOpenMode is not null)
            userPreferences.UpdateUIOpenMode(request.DefaultUIOpenMode);
        if (request.IsAutoStartEnabled is not null)
        {
            if (request.IsAutoStartEnabled.Value && !windowsStartupManager.IsEnabled())
                windowsStartupManager.Enable();
            else if (!request.IsAutoStartEnabled.Value && windowsStartupManager.IsEnabled())
                windowsStartupManager.Disable();
            userPreferences.UpdateAutoStart(request.IsAutoStartEnabled.Value);
        }
        if (request.IsSilentStartEnabled is not null)
            userPreferences.UpdateSilentStart(request.IsSilentStartEnabled.Value);
        if (request.Language is not null)
            userPreferences.UpdateLanguage(request.Language);
        if (request.ShouldDestroyWindowOnClose is not null)
            userPreferences.UpdateWindowDestroyOnClose(request.ShouldDestroyWindowOnClose.Value);

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