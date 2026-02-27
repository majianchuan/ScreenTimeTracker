using Mediator;

namespace ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;

public record GetUserSettingsQuery() : IRequest<GetUserSettingsResult>;