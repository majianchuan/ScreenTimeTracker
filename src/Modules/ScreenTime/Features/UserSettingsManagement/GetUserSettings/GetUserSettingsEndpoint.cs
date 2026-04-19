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
                AppIconDirectory: userSettings.AppIconDirectory,
                AppInfoStaleThresholdMinutes: (int)userSettings.AppInfoStaleThreshold.TotalMinutes,
                ActiveSessionAutoSaveSeconds: (int)userSettings.ActiveSessionAutoSaveInterval.TotalSeconds,

                IsIdleDetectionEnabled: userSettings.IsIdleDetectionEnabled,
                IdleThresholdSeconds: (int)userSettings.IdleThreshold.TotalSeconds,
                IdleDetectionPollingIntervalSeconds: (int)userSettings.IdleDetectionPollingInterval.TotalSeconds,

                MinValidSessionDurationSeconds: (int)userSettings.MinValidSessionDuration.TotalSeconds,
                SessionMergeToleranceSeconds: (int)userSettings.SessionMergeTolerance.TotalSeconds,
                SessionOptimizationIntervalSeconds: (int)userSettings.SessionOptimizationInterval.TotalSeconds,

                DayBoundaryOffsetHours: userSettings.DayBoundaryOffsetHours
            ),
            cancellationToken);
    }
}
