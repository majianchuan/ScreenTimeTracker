using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.PatchUserSettings;

public class PatchUserSettingsEndpoint(
    IMediator mediator
    ) : Endpoint<PatchUserSettingsRequest, EmptyResponse>
{
    public override void Configure()
    {
        Patch("user-settings");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(PatchUserSettingsRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new PatchUserSettingsCommand(
                SamplingInterval: req.SamplingIntervalMilliseconds is null ? null : TimeSpan.FromMilliseconds(req.SamplingIntervalMilliseconds.Value),
                IdleDetection: req.IdleDetection,
                IdleTimeout: req.IdleTimeoutSeconds is null ? null : TimeSpan.FromSeconds(req.IdleTimeoutSeconds.Value),
                AppInfoStaleThreshold: req.AppInfoStaleThresholdMinutes is null ? null : TimeSpan.FromMinutes(req.AppInfoStaleThresholdMinutes.Value),
                AggregationInterval: req.AggregationIntervalMinutes is null ? null : TimeSpan.FromMinutes(req.AggregationIntervalMinutes.Value),
                AppIconDirectory: req.AppIconDirectory
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
