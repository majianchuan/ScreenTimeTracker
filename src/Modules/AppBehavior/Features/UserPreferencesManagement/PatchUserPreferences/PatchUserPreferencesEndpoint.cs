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
                req.DefaultUIOpenMode,
                req.IsAutoStartEnabled,
                req.IsSilentStartEnabled,
                req.Language,
                req.ShouldDestroyWindowOnClose
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
