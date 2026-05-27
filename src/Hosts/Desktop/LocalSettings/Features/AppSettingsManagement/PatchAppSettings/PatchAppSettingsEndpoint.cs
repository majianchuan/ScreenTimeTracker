using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.PatchAppSettings;

public class PatchAppSettingsEndpoint(
    IMediator mediator
    ) : Endpoint<PatchAppSettingsRequest, EmptyResponse>
{
    public override void Configure()
    {
        Patch("app-settings");
        Group<DesktopGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatchAppSettingsRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new PatchAppSettingsCommand(
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
