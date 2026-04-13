using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.GetUserPreferences;

public class GetUserPreferencesEndpoint(
    IMediator mediator
    ) : Endpoint<EmptyRequest, GetUserPreferencesResponse>
{
    public override void Configure()
    {
        Get("user-preferences");
        Group<AppBehaviorGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken cancellationToken)
    {
        GetUserPreferencesResult userSettings = await mediator.Send(
            new GetUserPreferencesQuery(),
            cancellationToken
        );
        await Send.OkAsync(new GetUserPreferencesResponse(
            userSettings.UIOpenMode.ToString(),
            userSettings.AutoStart,
            userSettings.SilentStart,
            userSettings.Language,
            userSettings.WindowDestroyOnClose
        ), cancellationToken);
    }
}
