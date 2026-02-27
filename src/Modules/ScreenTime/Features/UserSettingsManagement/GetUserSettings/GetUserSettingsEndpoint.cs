using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.GetUserSettings;

public class GetUserSettingsEndpoint(
    IMediator mediator
    ) : Endpoint<EmptyRequest, GetUserSettingsResponse>
{
    public override void Configure()
    {
        Get("user-settings");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken cancellationToken)
    {
        GetUserSettingsResult userSettings = await mediator.Send(
            new GetUserSettingsQuery(),
            cancellationToken
        );
        await Send.OkAsync(
            new GetUserSettingsResponse(
                SamplingIntervalMilliseconds: (int)userSettings.SamplingInterval.TotalMilliseconds,
                IdleDetection: userSettings.IdleDetection,
                IdleTimeoutSeconds: (int)userSettings.IdleTimeout.TotalSeconds,
                AggregationIntervalMinutes: (int)userSettings.AggregationInterval.TotalMinutes,
                AppInfoStaleThresholdMinutes: (int)userSettings.AppInfoStaleThreshold.TotalMinutes,
                AppIconDirectory: userSettings.AppIconDirectory
            ),
            cancellationToken);
    }
}
