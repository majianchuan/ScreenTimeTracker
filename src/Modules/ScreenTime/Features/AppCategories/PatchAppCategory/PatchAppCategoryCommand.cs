using Mediator;
using ScreenTimeTracker.BuildingBlocks.Common.Types;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.PatchAppCategory;

public record PatchAppCategoryCommand(
    Guid Id,
    Optional<string> Name,
    Optional<string?> IconPath
) : IRequest;