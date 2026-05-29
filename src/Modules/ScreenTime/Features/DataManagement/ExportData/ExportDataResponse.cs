namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ExportData;

public record ExportDataResponse(
    ExportDataResponse.AppCategory[] AppCategories,
    ExportDataResponse.App[] Apps,
    ExportDataResponse.AppUsageSession[] AppUsageSessions
)
{
    public int Version { get; init; } = 2;

    public record Icon(string Extension, byte[] Data);

    public record AppCategory(string Name, Icon? Icon);

    public record App(
        string Name,
        string ProcessName,
        string AppCategoryName,
        Icon? Icon
    );

    public record AppUsageSession(
        string AppProcessName,
        string StartTime,
        string EndTime
    );
};

