using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Modules.AppBehavior.Domain;


public class UserPreferences : Entity
{
    public static readonly Guid DefaultId = Guid.Parse("00000000-0000-0000-0000-000000000001");


    public UIOpenMode DefaultUIOpenMode { get; private set; }
    public bool IsAutoStartEnabled { get; private set; }
    public bool IsSilentStartEnabled { get; private set; }
    public string Language { get; private set; }
    public bool ShouldDestroyWindowOnClose { get; private set; }

    // EF Core 构造函数
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private UserPreferences() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static UserPreferences CreateDefault() => new()
    {
        Id = DefaultId,
        DefaultUIOpenMode = UIOpenMode.Window,
        IsAutoStartEnabled = false,
        IsSilentStartEnabled = false,
        Language = "en-US",
        ShouldDestroyWindowOnClose = true,
    };

    public void UpdateUIOpenMode(UIOpenMode uiOpenMode) => DefaultUIOpenMode = uiOpenMode;
    public void UpdateUIOpenMode(string uiOpenMode)
    {
        // 忽略大小写
        if (Enum.TryParse<UIOpenMode>(uiOpenMode, true, out var parsedUIOpenMode))
            DefaultUIOpenMode = parsedUIOpenMode;
    }

    public void UpdateAutoStart(bool autoStart) => IsAutoStartEnabled = autoStart;

    public void UpdateSilentStart(bool silentStart) => IsSilentStartEnabled = silentStart;

    public void UpdateLanguage(string language) => Language = language;

    public void UpdateWindowDestroyOnClose(bool windowDestroyOnClose) => ShouldDestroyWindowOnClose = windowDestroyOnClose;
}

public enum UIOpenMode
{
    Window,
    Browser
}