using Mediator;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.CreateAppCategory;

public class CreateAppCategoryHandler(
    ScreenTimeDbContext context
    ) : IRequestHandler<CreateAppCategoryCommand, CreateAppCategoryResponse>
{
    public async ValueTask<CreateAppCategoryResponse> Handle(CreateAppCategoryCommand request, CancellationToken cancellationToken)
    {
        AppCategory appCategory = AppCategory.Create(request.Name, request.IconPath);
        context.AppCategories.Add(appCategory);
        await context.SaveChangesAsync(cancellationToken);
        return new CreateAppCategoryResponse(
            Id: appCategory.Id,
            Name: appCategory.Name,
            IconPath: appCategory.IconPath,
            IsSystem: appCategory.IsSystem
        );
    }
}