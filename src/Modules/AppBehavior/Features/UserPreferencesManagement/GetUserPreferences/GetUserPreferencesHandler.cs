using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.AppBehavior.Domain;
using ScreenTimeTracker.Modules.AppBehavior.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.GetUserPreferences;

public class GetUserPreferencesHandler(
    AppBehaviorDbContext context
    ) : IRequestHandler<GetUserPreferencesQuery, GetUserPreferencesResult>
{
    public async ValueTask<GetUserPreferencesResult> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
    {
        UserPreferences userPreferences = await context.UserPreferences.AsNoTracking().SingleAsync(cancellationToken);


        return new GetUserPreferencesResult(
            userPreferences.UIOpenMode switch
            {
                UIOpenMode.Window => UIOpenModeDto.Window,
                UIOpenMode.Browser => UIOpenModeDto.Browser,
                _ => throw new ArgumentOutOfRangeException(nameof(request),
                    userPreferences.UIOpenMode,
                    "Unhandled UIOpenMode value")
            },
            userPreferences.AutoStart,
            userPreferences.SilentStart,
            userPreferences.Language,
            userPreferences.WindowDestroyOnClose
        );
    }
}