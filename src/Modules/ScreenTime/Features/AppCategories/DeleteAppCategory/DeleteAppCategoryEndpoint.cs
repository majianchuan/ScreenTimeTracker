using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.DeleteAppCategory;

public class DeleteAppCategoryEndpoint(
    IMediator mediator
    ) : Endpoint<DeleteAppCategoryRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("app-categories/{appCategoryId}");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteAppCategoryRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeleteAppCategoryCommand(
                AppCategoryId: req.AppCategoryId
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
