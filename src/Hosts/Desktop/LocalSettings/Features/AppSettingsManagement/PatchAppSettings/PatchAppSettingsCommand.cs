using Mediator;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.PatchAppSettings;

public record PatchAppSettingsCommand(
    string? DefaultUIOpenMode,
    bool? IsAutoStartEnabled,
    bool? IsSilentStartEnabled,
    string? Language,
    bool? ShouldDestroyWindowOnClose
) : IRequest;

