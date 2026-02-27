using Mediator;

namespace ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.PatchUserSettings;

public record PatchUserSettingsCommand(
    string? UIOpenMode,
    bool? AutoStart,
    bool? SilentStart,
    string? Language,
    bool? WindowDestroyOnClose
) : IRequest;

