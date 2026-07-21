using ScreenTimeTracker.BuildingBlocks.Domain;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain.Events;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;

public class AppSettings : Entity
{
    public static readonly Guid DefaultId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public static readonly IReadOnlySet<string> SupportedLanguages = new HashSet<string>()
    {
        "en-US",
        "zh-CN"
    };

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

    public void UpdateDefaultUIOpenMode(UIOpenMode defaultUIOpenMode)
    {
        if (defaultUIOpenMode != DefaultUIOpenMode)
        {
            DefaultUIOpenMode = defaultUIOpenMode;
            AddDomainEvent(new AppSettingsUpdatedDomainEvent(DefaultUIOpenMode, IsAutoStartEnabled, IsSilentStartEnabled, Language));
        }
    }

    public void UpdateDefaultUIOpenMode(string defaultUIOpenMode)
    {
        // 忽略大小写
        if (Enum.TryParse<UIOpenMode>(defaultUIOpenMode, true, out var parsedUIOpenMode) && parsedUIOpenMode != DefaultUIOpenMode)
        {
            DefaultUIOpenMode = parsedUIOpenMode;
            AddDomainEvent(new AppSettingsUpdatedDomainEvent(DefaultUIOpenMode, IsAutoStartEnabled, IsSilentStartEnabled, Language));
        }
    }

    public void UpdateIsAutoStartEnabled(bool isAutoStartEnabled)
    {
        if (isAutoStartEnabled != IsAutoStartEnabled)
        {
            IsAutoStartEnabled = isAutoStartEnabled;
            AddDomainEvent(new AppSettingsUpdatedDomainEvent(DefaultUIOpenMode, IsAutoStartEnabled, IsSilentStartEnabled, Language));
        }
    }

    public void UpdateIsSilentStartEnabled(bool isSilentStartEnabled)
    {
        if (isSilentStartEnabled != IsSilentStartEnabled)
        {
            IsSilentStartEnabled = isSilentStartEnabled;
            AddDomainEvent(new AppSettingsUpdatedDomainEvent(DefaultUIOpenMode, IsAutoStartEnabled, IsSilentStartEnabled, Language));
        }
    }

    public void UpdateLanguage(string language)
    {
        if (!SupportedLanguages.Contains(language))
        {
            throw new ArgumentException($"Unsupported language: {language}", nameof(language));
        }
        if (language != Language)
        {
            Language = language;
            AddDomainEvent(new AppSettingsUpdatedDomainEvent(DefaultUIOpenMode, IsAutoStartEnabled, IsSilentStartEnabled, Language));
        }
    }
}

public enum UIOpenMode
{
    Window,
    Browser
}