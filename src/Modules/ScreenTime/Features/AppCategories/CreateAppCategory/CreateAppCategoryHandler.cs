using Mediator;
using Microsoft.EntityFrameworkCore;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.CreateAppCategory;

public class CreateAppCategoryHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<CreateAppCategoryCommand, CreateAppCategoryResponse>
{
    public async ValueTask<CreateAppCategoryResponse> Handle(CreateAppCategoryCommand request, CancellationToken cancellationToken)
    {
        var exists = await context.AppCategories
            .AnyAsync(x => x.Name == request.Name, cancellationToken);
        if (exists)
            throw new Exception("App category with the same name already exists.");

        AppCategory appCategory = AppCategory.Create(request.Name, request.Color, request.IconPath);
        context.AppCategories.Add(appCategory);
        await context.SaveChangesAsync(cancellationToken);
        return new CreateAppCategoryResponse(
            appCategory.Id,
            appCategory.Name,
            appCategory.Color,
            appCategory.IconPath,
            appCategory.IsSystem
        );
    }
}