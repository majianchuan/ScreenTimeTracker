using ScreenTimeTracker.BuildingBlocks.Domain;

namespace ScreenTimeTracker.Hosts.Desktop.LocalSettings.Domain.Events;

public record AppSettingsUpdatedDomainEvent(
    UIOpenMode DefaultUIOpenMode,
    bool IsAutoStartEnabled,
    bool IsSilentStartEnabled,
    string Language
) : IDomainEvent;
