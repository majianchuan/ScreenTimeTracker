using Mediator;

namespace ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.GetUserPreferences;

public record GetUserPreferencesQuery() : IRequest<GetUserPreferencesResult>;