using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public class App : Entity
{
    public static readonly Guid IdleAppId = new("00000000-0000-0000-0000-000000000001");
    public static readonly Guid UnknownAppId = new("00000000-0000-0000-0000-000000000002");

    public string Name { get; private set; }
    public string ProcessName { get; private set; }
    public bool IsAutoUpdateEnabled { get; private set; }
    public DateTime LastAutoUpdated { get; private set; }
    public Guid AppCategoryId { get; private set; }
    public AppCategory? AppCategory { get; private set; }
    public string? ExecutablePath { get; private set; }
    public string? IconPath { get; private set; }
    public string? Description { get; private set; }
    public bool IsSystem { get; private set; }

    // EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private App() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static App Create(
        DateTime now,
        string name,
        string executableName,
        bool autoUpdate = true,
        string? executablePath = null,
        string? iconPath = null,
        string? description = null
    )
    {
        return new App()
        {
            Name = name,
            ProcessName = executableName,
            IsAutoUpdateEnabled = autoUpdate,
            LastAutoUpdated = now,
            AppCategoryId = AppCategory.UncategorizedId,
            ExecutablePath = executablePath,
            IconPath = iconPath,
            Description = description,
            IsSystem = false
        };
    }

    public static App CreateIdleApp() =>
        new()
        {
            Id = IdleAppId,
            Name = "Idle",
            ProcessName = "Idle",
            IsAutoUpdateEnabled = false,
            LastAutoUpdated = DateTime.MinValue,
            AppCategoryId = AppCategory.UncategorizedId,
            ExecutablePath = null,
            IconPath = null,
            Description = "Shows when the user is idle",
            IsSystem = true
        };

    public static App CreateUnknownApp() =>
        new()
        {
            Id = UnknownAppId,
            Name = "Unknown",
            ProcessName = "Unknown",
            IsAutoUpdateEnabled = false,
            LastAutoUpdated = DateTime.MinValue,
            AppCategoryId = AppCategory.UncategorizedId,
            ExecutablePath = null,
            IconPath = null,
            Description = "Represents unknown or restricted apps",
            IsSystem = true
        };

    public void UpdateName(string name) => Name = name;
    public void UpdateIsAutoUpdateEnabled(bool isAutoUpdateEnabled) => IsAutoUpdateEnabled = isAutoUpdateEnabled;
    public void UpdateAppCategoryId(Guid categoryId) => AppCategoryId = categoryId;
    public void UpdateIconPath(string? iconPath) => IconPath = iconPath;

    public void UpdateSystemDetails(DateTime now, string? executablePath, string? iconPath, string? description)
    {
        if (!IsAutoUpdateEnabled) return;

        ExecutablePath = executablePath;
        IconPath = iconPath;
        Description = description;
        LastAutoUpdated = now;
    }

    public bool NeedsUpdate(DateTime now, TimeSpan threshold)
    {
        return IsAutoUpdateEnabled && (now - LastAutoUpdated) >= threshold;
    }
}
