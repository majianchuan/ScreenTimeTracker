using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.GetAppSettings;

public record GetAppSettingsResult(
    UIOpenMode DefaultUIOpenMode,
    bool IsAutoStartEnabled,
    bool IsSilentStartEnabled,
    string Language
);
