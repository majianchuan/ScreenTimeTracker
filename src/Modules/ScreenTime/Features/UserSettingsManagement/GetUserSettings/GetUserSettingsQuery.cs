using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.UserSettingsManagement.GetUserSettings;

public record GetUserSettingsQuery() : IRequest<GetUserSettingsResult>;