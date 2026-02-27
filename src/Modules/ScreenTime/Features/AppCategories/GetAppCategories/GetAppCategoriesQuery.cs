using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategories;

public record GetAppCategoriesQuery(
    string? Fields // 逗号分隔的字段列表
) : IRequest<List<Dictionary<string, object?>>>;
