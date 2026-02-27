using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.DeleteAppCategory;

public record DeleteAppCategoryCommand(
    Guid Id
) : IRequest;