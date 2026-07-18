using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Domain;


public class AppCategory : Entity
{
    public static readonly Guid UncategorizedId = new("00000000-0000-0000-0000-000000000001");
    public string Name { get; private set; }
    public string Color { get; private set; }
    public string? IconPath { get; private set; }
    public DateTime IconLastUpdatedAt { get; private set; }
    public bool IsSystem { get; private set; }

    // EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private AppCategory() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static AppCategory Create(
        DateTime now,
        string name,
        string color,
        string? iconPath = null)
    {
        return new AppCategory
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Color = color,
            IconPath = iconPath,
            IconLastUpdatedAt = now,
            IsSystem = false,
        };
    }

    public static AppCategory Rehydrate(string name, string color, string? iconPath, DateTime iconLastUpdatedAt, bool isSystem)
    {
        return new AppCategory
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            Color = color,
            IconPath = iconPath,
            IconLastUpdatedAt = iconLastUpdatedAt,
            IsSystem = isSystem,
        };
    }

    public static AppCategory CreateUncategorized() => new()
    {
        Id = UncategorizedId,
        Name = "Uncategorized",
        Color = "#8E8E93",
        IconPath = null,
        IconLastUpdatedAt = DateTime.MinValue,
        IsSystem = true
    };

    public void UpdateName(string name) => Name = name;
    public void UpdateColor(string color) => Color = color;
    public void UpdateIconPath(string? iconPath, DateTime now)
    {
        IconPath = iconPath;
        IconLastUpdatedAt = now;
    }
}
