using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.GetAppSettings;

public class GetAppSettingsEndpoint(
    IMediator mediator
    ) : Endpoint<EmptyRequest, GetAppSettingsResult>
{
    public override void Configure()
    {
        Get("app-settings");
        Group<LocalSettingsGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken cancellationToken)
    {
        GetAppSettingsResult response = await mediator.Send(
            new GetAppSettingsQuery(),
            cancellationToken
        );
        await Send.OkAsync(response, cancellationToken);
    }
}
