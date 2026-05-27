using Mediator;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.GetAppSettings;

public record GetAppSettingsQuery() : IRequest<GetAppSettingsResult>;