using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.PatchUserPreferences;

public class PatchUserPreferencesEndpoint(
    IMediator mediator
    ) : Endpoint<PatchUserPreferencesRequest, EmptyResponse>
{
    public override void Configure()
    {
        Patch("user-preferences");
        Group<AppBehaviorGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatchUserPreferencesRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new PatchUserPreferencesCommand(
                req.UIOpenMode,
                req.AutoStart,
                req.SilentStart,
                req.Language,
                req.WindowDestroyOnClose
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
