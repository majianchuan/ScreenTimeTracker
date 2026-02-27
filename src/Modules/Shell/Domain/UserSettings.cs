using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Modules.Shell.Domain;


public class UserSettings : Entity
{
    public static readonly Guid DefaultId = Guid.Parse("00000000-0000-0000-0000-000000000001");


    public UIOpenMode UIOpenMode { get; private set; }
    public bool AutoStart { get; private set; }
    public bool SilentStart { get; private set; }
    public string Language { get; private set; }
    public bool WindowDestroyOnClose { get; private set; }

    // EF Core 构造函数
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private UserSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static UserSettings CreateDefault() => new()
    {
        Id = DefaultId,
        UIOpenMode = UIOpenMode.Window,
        AutoStart = false,
        SilentStart = false,
        Language = "en-US",
        WindowDestroyOnClose = false,
    };

    public void UpdateUIOpenMode(UIOpenMode uiOpenMode) => UIOpenMode = uiOpenMode;
    public void UpdateUIOpenMode(string uiOpenMode)
    {
        // 忽略大小写
        if (Enum.TryParse<UIOpenMode>(uiOpenMode, true, out var parsedUIOpenMode))
            UIOpenMode = parsedUIOpenMode;
    }

    public void UpdateAutoStart(bool autoStart) => AutoStart = autoStart;

    public void UpdateSilentStart(bool silentStart) => SilentStart = silentStart;

    public void UpdateLanguage(string language) => Language = language;

    public void UpdateWindowDestroyOnClose(bool windowDestroyOnClose) => WindowDestroyOnClose = windowDestroyOnClose;
}

public enum UIOpenMode
{
    Window,
    Browser
}