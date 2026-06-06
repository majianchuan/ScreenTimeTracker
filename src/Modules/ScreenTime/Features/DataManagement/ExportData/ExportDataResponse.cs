namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ExportData;

public record ExportDataResponse(
    ExportDataResponse.AppCategory[] AppCategories,
    ExportDataResponse.App[] Apps,
    ExportDataResponse.AppUsageSession[] AppUsageSessions
)
{
    public int Version { get; init; } = 3;

    public record Icon(string Extension, byte[] Data);

    public record AppCategory(string Name, string Color, Icon? Icon);

    public record App(
        string Name,
        string Color,
        string ProcessName,
        bool IsAutoUpdateEnabled,
        string AppCategoryName,
        Icon? Icon
    );

    public record AppUsageSession(
        string AppProcessName,
        string StartTime,
        string EndTime
    );
};

