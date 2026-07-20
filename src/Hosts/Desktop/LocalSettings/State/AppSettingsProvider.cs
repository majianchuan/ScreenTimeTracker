using Mediator;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain.Events;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.State;

public interface IAppSettingsProvider
{
    UIOpenMode DefaultUIOpenMode { get; }
    bool IsAutoStartEnabled { get; }
    bool IsSilentStartEnabled { get; }
    string Language { get; }

    event EventHandler<SettingChangedEventArgs>? OnSettingChanged;

    void Update(UIOpenMode defaultUIOpenMod, bool isAutoStartEnabled, bool isSilentStartEnabled, string language);
}

public class SettingChangedEventArgs(string settingName) : EventArgs
{
    public string SettingName { get; } = settingName;
}

public class AppSettingsProvider : IAppSettingsProvider
{
    public UIOpenMode DefaultUIOpenMode { get; private set; } = UIOpenMode.Window;
    public bool IsAutoStartEnabled { get; private set; } = false;
    public bool IsSilentStartEnabled { get; private set; } = false;
    public string Language { get; private set; } = "en-US";

    public event EventHandler<SettingChangedEventArgs>? OnSettingChanged;

    internal void Initialize(UIOpenMode defaultUIOpenMode, bool isAutoStartEnabled,
       bool isSilentStartEnabled, string language)
    {
        DefaultUIOpenMode = defaultUIOpenMode;
        IsAutoStartEnabled = isAutoStartEnabled;
        IsSilentStartEnabled = isSilentStartEnabled;
        Language = language;
    }

    public void Update(UIOpenMode defaultUIOpenMode, bool isAutoStartEnabled,
        bool isSilentStartEnabled, string language)
    {
        if (defaultUIOpenMode != DefaultUIOpenMode)
        {
            DefaultUIOpenMode = defaultUIOpenMode;
            OnSettingChanged?.Invoke(this, new SettingChangedEventArgs(nameof(DefaultUIOpenMode)));
        }
        if (isAutoStartEnabled != IsAutoStartEnabled)
        {
            IsAutoStartEnabled = isAutoStartEnabled;
            OnSettingChanged?.Invoke(this, new SettingChangedEventArgs(nameof(IsAutoStartEnabled)));
        }
        if (isSilentStartEnabled != IsSilentStartEnabled)
        {
            IsSilentStartEnabled = isSilentStartEnabled;
            OnSettingChanged?.Invoke(this, new SettingChangedEventArgs(nameof(IsSilentStartEnabled)));
        }
        if (language != Language)
        {
            Language = language;
            OnSettingChanged?.Invoke(this, new SettingChangedEventArgs(nameof(Language)));
        }
    }
}

public class AppSettingsChangedHandler(IAppSettingsProvider appSettingsProvider) : INotificationHandler<AppSettingsUpdatedDomainEvent>
{
    public ValueTask Handle(AppSettingsUpdatedDomainEvent notification, CancellationToken cancellationToken)
    {
        appSettingsProvider.Update(notification.DefaultUIOpenMode, notification.IsAutoStartEnabled, notification.IsSilentStartEnabled, notification.Language);
        return ValueTask.CompletedTask;
    }
}