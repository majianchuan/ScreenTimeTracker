namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ExportData;

public record ExportDataResponse(
    UsageSession[] UsageSessions
)
{
    public int Version { get; init; } = 1;
};

public record UsageSession(
    string AppName,
    string AppProcessName,
    string StartTime,
    string EndTime
);