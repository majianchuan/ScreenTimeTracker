using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public class App : Entity
{
    public static readonly Guid IdleAppId = new("00000000-0000-0000-0000-000000000001");
    public static readonly Guid UnknownAppId = new("00000000-0000-0000-0000-000000000002");

    public string Name { get; private set; }
    public string Color { get; private set; }
    public string ProcessName { get; private set; }
    public bool IsAutoUpdateEnabled { get; private set; }
    public DateTime LastAutoUpdatedAt { get; private set; }
    public Guid AppCategoryId { get; private set; }
    public AppCategory? AppCategory { get; private set; }
    public string? ExecutablePath { get; private set; }
    public string? IconPath { get; private set; }
    public DateTime IconLastUpdatedAt { get; private set; }
    public bool IsSystem { get; private set; }

    // EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private App() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    public static App Create(
        DateTime now,
        string name,
        string processName,
        bool autoUpdate = true,
        string? executablePath = null,
        string? iconPath = null)
    {
        return new App()
        {
            Name = name,
            Color = GenerateColor(),
            ProcessName = processName,
            IsAutoUpdateEnabled = autoUpdate,
            LastAutoUpdatedAt = now,
            AppCategoryId = AppCategory.UncategorizedId,
            ExecutablePath = executablePath,
            IconPath = iconPath,
            IconLastUpdatedAt = now,
            IsSystem = false
        };
    }

    public static App Rehydrate(
        string name,
        string color,
        string processName,
        bool isAutoUpdateEnabled,
        DateTime lastAutoUpdated,
        Guid appCategoryId,
        string? executablePath,
        string? iconPath,
        DateTime iconLastUpdatedAt,
        bool isSystem)
    {
        return new App
        {
            Name = name,
            Color = color,
            ProcessName = processName,
            IsAutoUpdateEnabled = isAutoUpdateEnabled,
            LastAutoUpdatedAt = lastAutoUpdated,
            AppCategoryId = appCategoryId,
            ExecutablePath = executablePath,
            IconPath = iconPath,
            IconLastUpdatedAt = iconLastUpdatedAt,
            IsSystem = isSystem
        };
    }

    public static App CreateIdleApp() =>
        new()
        {
            Id = IdleAppId,
            Name = "Idle",
            Color = "#C7C7CC",
            ProcessName = "Idle",
            IsAutoUpdateEnabled = false,
            LastAutoUpdatedAt = DateTime.MinValue,
            AppCategoryId = AppCategory.UncategorizedId,
            ExecutablePath = null,
            IconPath = null,
            IconLastUpdatedAt = DateTime.MinValue,
            IsSystem = true
        };

    public static App CreateUnknownApp() =>
        new()
        {
            Id = UnknownAppId,
            Name = "Unknown",
            Color = "#636366",
            ProcessName = "Unknown",
            IsAutoUpdateEnabled = false,
            LastAutoUpdatedAt = DateTime.MinValue,
            AppCategoryId = AppCategory.UncategorizedId,
            ExecutablePath = null,
            IconPath = null,
            IconLastUpdatedAt = DateTime.MinValue,
            IsSystem = true
        };

    public void UpdateName(string name) => Name = name;
    public void UpdateColor(string color) => Color = color;
    public void UpdateIsAutoUpdateEnabled(bool isAutoUpdateEnabled) => IsAutoUpdateEnabled = isAutoUpdateEnabled;
    public void UpdateAppCategoryId(Guid categoryId) => AppCategoryId = categoryId;
    public void UpdateIconPath(string? iconPath, DateTime now)
    {
        IconPath = iconPath;
        IconLastUpdatedAt = now;
    }

    public void UpdateSystemDetails(DateTime now, string? executablePath, string? iconPath, string? description)
    {
        if (!IsAutoUpdateEnabled) return;

        ExecutablePath = executablePath;
        IconPath = iconPath;
        LastAutoUpdatedAt = now;
    }

    public bool NeedsUpdate(DateTime now, TimeSpan threshold)
    {
        return IsAutoUpdateEnabled && (now - LastAutoUpdatedAt) >= threshold;
    }

    private static string HslToHex(double h, double s, double l)
    {
        h %= 360;
        s /= 100.0;
        l /= 100.0;

        double c = (1 - Math.Abs(2 * l - 1)) * s;
        double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
        double m = l - c / 2;

        double r1 = 0, g1 = 0, b1 = 0;

        if (h < 60)
            (r1, g1, b1) = (c, x, 0);
        else if (h < 120)
            (r1, g1, b1) = (x, c, 0);
        else if (h < 180)
            (r1, g1, b1) = (0, c, x);
        else if (h < 240)
            (r1, g1, b1) = (0, x, c);
        else if (h < 300)
            (r1, g1, b1) = (x, 0, c);
        else
            (r1, g1, b1) = (c, 0, x);

        int r = (int)Math.Round((r1 + m) * 255);
        int g = (int)Math.Round((g1 + m) * 255);
        int b = (int)Math.Round((b1 + m) * 255);

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static string GenerateColor()
    {
        int h = Random.Shared.Next(360);
        int s = Random.Shared.Next(50, 80);
        int l = Random.Shared.Next(50, 80);
        return HslToHex(h, s, l);
    }
}
