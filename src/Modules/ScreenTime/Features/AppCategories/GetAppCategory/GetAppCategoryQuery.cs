using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategory;

public record GetAppCategoryQuery(
    Guid AppCategoryId
) : IRequest<GetAppCategoryResponse?>;
