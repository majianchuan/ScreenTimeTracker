using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.Shell.Domain;
using ScreenTimeTracker.Modules.Shell.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;

public class GetUserSettingsHandler(
    ShellDbContext context
    ) : IRequestHandler<GetUserSettingsQuery, GetUserSettingsResult>
{
    public async ValueTask<GetUserSettingsResult> Handle(GetUserSettingsQuery request, CancellationToken cancellationToken)
    {
        UserSettings userSettings = await context.UserSettings.AsNoTracking().SingleAsync(cancellationToken);


        return new GetUserSettingsResult(
            userSettings.UIOpenMode switch
            {
                UIOpenMode.Window => UIOpenModeDto.Window,
                UIOpenMode.Browser => UIOpenModeDto.Browser,
                _ => throw new ArgumentOutOfRangeException(nameof(request),
                    userSettings.UIOpenMode,
                    "Unhandled UIOpenMode value")
            },
            userSettings.AutoStart,
            userSettings.SilentStart,
            userSettings.Language,
            userSettings.WindowDestroyOnClose
        );
    }
}