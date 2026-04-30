using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategory;

public class GetAppCategoryEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppCategoryRequest, GetAppCategoryResponse>
{
    public override void Configure()
    {
        Get("app-categories/{appCategoryId}");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppCategoryRequest req, CancellationToken cancellationToken)
    {
        var appCategory = await mediator.Send(
            new GetAppCategoryQuery(req.AppCategoryId),
            cancellationToken
        );

        if (appCategory is null)
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        await Send.OkAsync(appCategory, cancellationToken);
    }
}
