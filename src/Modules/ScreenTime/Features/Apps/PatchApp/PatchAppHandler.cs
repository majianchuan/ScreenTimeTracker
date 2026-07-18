using Mediator;
using ScreenTimeTracker.Modules.ScreenTime.Domain;
using ScreenTimeTracker.Modules.ScreenTime.Infrastructure.Persistence;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.PatchApp;

public class PatchAppHandler(
    ScreenTimeDbContext context,
    TimeProvider timeProvider
    ) : IRequestHandler<PatchAppCommand>
{
    public async ValueTask<Unit> Handle(PatchAppCommand request, CancellationToken cancellationToken)
    {
        App? App = await context.Apps.FindAsync([request.Id], cancellationToken);
        if (App is null)
            return Unit.Value;

        if (request.Name.HasValue)
            App.UpdateName(request.Name.Value);
        if (request.Color.HasValue)
            App.UpdateColor(request.Color.Value);
        if (request.IsAutoUpdateEnabled.HasValue)
            App.UpdateIsAutoUpdateEnabled(request.IsAutoUpdateEnabled.Value);
        if (request.AppCategoryId.HasValue)
            App.UpdateAppCategoryId(request.AppCategoryId.Value);
        if (request.IconPath.HasValue)
            App.UpdateIconPath(request.IconPath.Value, timeProvider.GetLocalNow().DateTime);

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}