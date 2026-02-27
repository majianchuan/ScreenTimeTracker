using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryIcon;

public class GetAppCategoryIconPathHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<GetAppCategoryIconPathQuery, string?>
{
    public async ValueTask<string?> Handle(GetAppCategoryIconPathQuery request, CancellationToken cancellationToken)
    {
        var appCategory = await context.AppCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                appCategory => appCategory.Id == request.Id,
                cancellationToken
            );
        return appCategory?.IconPath;
    }
}