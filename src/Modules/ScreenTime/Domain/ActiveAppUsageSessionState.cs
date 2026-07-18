namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public record ActiveAppUsageSessionState(
    Guid AppId,
    DateTime StartTime
);