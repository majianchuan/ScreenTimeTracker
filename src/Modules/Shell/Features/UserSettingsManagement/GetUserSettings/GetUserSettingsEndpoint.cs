using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;

public class GetUserSettingsEndpoint(
    IMediator mediator
    ) : Endpoint<EmptyRequest, GetUserSettingsResult>
{
    public override void Configure()
    {
        Get("user-settings");
        Group<ShellGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken cancellationToken)
    {
        GetUserSettingsResult userSettings = await mediator.Send(
            new GetUserSettingsQuery(),
            cancellationToken
        );
        await Send.OkAsync(userSettings, cancellationToken);
    }
}
