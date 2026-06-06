using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.CreateAppCategory;

public record CreateAppCategoryCommand(
    string Name,
    string Color,
    string? IconPath
) : IRequest<CreateAppCategoryResponse>;
