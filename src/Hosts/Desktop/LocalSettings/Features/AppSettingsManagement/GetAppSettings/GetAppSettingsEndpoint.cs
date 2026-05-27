using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.GetAppSettings;

public class GetAppSettingsEndpoint(
    IMediator mediator
    ) : Endpoint<EmptyRequest, GetAppSettingsResponse>
{
    public override void Configure()
    {
        Get("app-settings");
        Group<DesktopGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken cancellationToken)
    {
        GetAppSettingsResult userSettings = await mediator.Send(
            new GetAppSettingsQuery(),
            cancellationToken
        );
        await Send.OkAsync(new GetAppSettingsResponse(
            userSettings.DefaultUIOpenMode.ToString(),
            userSettings.IsAutoStartEnabled,
            userSettings.IsSilentStartEnabled,
            userSettings.Language,
            userSettings.ShouldDestroyWindowOnClose
        ), cancellationToken);
    }
}
