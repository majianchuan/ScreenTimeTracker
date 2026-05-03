namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ImportData;

public record ImportDataResponse(
    long ImportedCount,
    long SkippedCount
);