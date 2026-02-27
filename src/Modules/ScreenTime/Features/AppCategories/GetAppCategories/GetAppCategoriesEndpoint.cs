using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategories;

public class GetAppCategoriesEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppCategoriesRequest, List<Dictionary<string, object?>>>
{
    public override void Configure()
    {
        Get("app-categories");
        Group<ScreenTimeGroup>();
        AllowAnonymous();

        Description(d => d
            .Produces<List<GetAppCategoriesResponseItem>>(200, "application/json")
        );
    }

    public override async Task HandleAsync(GetAppCategoriesRequest req, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetAppCategoriesQuery(
                req.Fields
            ),
            cancellationToken
        );
        await Send.OkAsync(result, cancellationToken);
    }
}
