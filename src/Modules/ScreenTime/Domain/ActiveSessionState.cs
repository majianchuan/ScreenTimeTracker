namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public record ActiveSessionState(
    Guid AppId,
    DateTime StartTime
);