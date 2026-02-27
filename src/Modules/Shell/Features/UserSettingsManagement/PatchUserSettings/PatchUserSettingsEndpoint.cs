using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.PatchUserSettings;

public class PatchUserSettingsEndpoint(
    IMediator mediator
    ) : Endpoint<PatchUserSettingsRequest, EmptyResponse>
{
    public override void Configure()
    {
        Patch("user-settings");
        Group<ShellGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatchUserSettingsRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new PatchUserSettingsCommand(
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
