using Mediator;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.PatchAppCategory;

public class PatchAppCategoryHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<PatchAppCategoryCommand>
{
    public async ValueTask<Unit> Handle(PatchAppCategoryCommand request, CancellationToken cancellationToken)
    {
        AppCategory? appCategory = await context.AppCategories.FindAsync([request.AppCategoryId], cancellationToken);
        if (appCategory is null)
            return Unit.Value;

        if (request.Name.HasValue)
            appCategory.UpdateName(request.Name.Value);
        if (request.IconPath.HasValue)
            appCategory.UpdateIconPath(request.IconPath.Value);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}