using Mediator;
using ScreenTimeTracker.BuildingBlocks.Common.Types;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.PatchApp;

public record PatchAppCommand(
    Guid Id,
    Optional<string> Name,
    Optional<bool> IsAutoUpdateEnabled,
    Optional<Guid> AppCategoryId,
    Optional<string?> IconPath
) : IRequest;