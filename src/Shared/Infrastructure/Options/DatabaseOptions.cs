namespace ScreenTimeTracker.Shared.Infrastructure.Options;

public class DatabaseOptions
{
    public static readonly string SectionName = "DatabaseOptions";
    public string DBFilePath { get; set; } = "./Data/ScreenTimeTracker.db";
}
