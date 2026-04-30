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
                AppIconDirectory: req.AppIconDirectory,
                AppInfoStaleThreshold: req.AppInfoStaleThresholdMinutes is null ? null : TimeSpan.FromMinutes(req.AppInfoStaleThresholdMinutes.Value),
                ActiveSessionAutoSaveInterval: req.ActiveSessionAutoSaveSeconds is null ? null : TimeSpan.FromSeconds(req.ActiveSessionAutoSaveSeconds.Value),

                IsIdleDetectionEnabled: req.IsIdleDetectionEnabled,
                IdleThreshold: req.IdleThresholdSeconds is null ? null : TimeSpan.FromSeconds(req.IdleThresholdSeconds.Value),
                IdleDetectionPollingInterval: req.IdleDetectionPollingIntervalSeconds is null ? null : TimeSpan.FromSeconds(req.IdleDetectionPollingIntervalSeconds.Value),

                MinValidSessionDuration: req.MinValidSessionDurationSeconds is null ? null : TimeSpan.FromSeconds(req.MinValidSessionDurationSeconds.Value),
                SessionMergeTolerance: req.SessionMergeToleranceSeconds is null ? null : TimeSpan.FromSeconds(req.SessionMergeToleranceSeconds.Value),
                SessionOptimizationInterval: req.SessionOptimizationIntervalMinutes is null ? null : TimeSpan.FromMinutes(req.SessionOptimizationIntervalMinutes.Value),

                DayBoundaryOffsetHours: req.DayCutoffHour
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
