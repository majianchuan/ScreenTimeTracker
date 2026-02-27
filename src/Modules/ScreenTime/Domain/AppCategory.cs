using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Domain;


public class AppCategory : Entity
{
    public static readonly Guid UncategorizedId = new("00000000-0000-0000-0000-000000000001");
    public string Name { get; private set; }
    public string? IconPath { get; private set; }
    public bool IsSystem { get; private set; }

    // EF Core
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private AppCategory() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static AppCategory Create(string name, string? iconPath = null)
    {
        return new AppCategory
        {
            Id = Guid.CreateVersion7(),
            Name = name,
            IconPath = iconPath,
            IsSystem = false,
        };
    }

    public static AppCategory CreateUncategorized() => new()
    {
        Id = UncategorizedId,
        Name = "Uncategorized",
        IconPath = null,
        IsSystem = true
    };

    public void UpdateName(string name) => Name = name;

    public void UpdateIconPath(string? iconPath) => IconPath = iconPath;
}