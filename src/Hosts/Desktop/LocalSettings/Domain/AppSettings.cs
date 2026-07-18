using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;


public class AppSettings : Entity
{
    public static readonly Guid DefaultId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public UIOpenMode DefaultUIOpenMode { get; private set; }
    public bool IsAutoStartEnabled { get; private set; }
    public bool IsSilentStartEnabled { get; private set; }
    public string Language { get; private set; }

    // EF Core 构造函数
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private AppSettings() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public static AppSettings CreateDefault() => new()
    {
        Id = DefaultId,
        DefaultUIOpenMode = UIOpenMode.Window,
        IsAutoStartEnabled = false,
        IsSilentStartEnabled = false,
        Language = "en-US",
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
}

public enum UIOpenMode
{
    Window,
    Browser
}