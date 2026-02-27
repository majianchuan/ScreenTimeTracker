using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.DeleteAppCategory;

public class DeleteAppCategoryHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<DeleteAppCategoryCommand>
{
    public async ValueTask<Unit> Handle(DeleteAppCategoryCommand request, CancellationToken cancellationToken)
    {
        AppCategory? appCategory = await context.AppCategories.FindAsync([request.Id], cancellationToken);
        if (appCategory is null || appCategory.IsSystem)
            return Unit.Value;

        // 把所有这个类别的 App 都设置为默认类别
        await context.Apps
            .Where(app => app.AppCategoryId == request.Id)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(app => app.AppCategoryId, AppCategory.UncategorizedId),
                cancellationToken
            );

        context.AppCategories.Remove(appCategory);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}