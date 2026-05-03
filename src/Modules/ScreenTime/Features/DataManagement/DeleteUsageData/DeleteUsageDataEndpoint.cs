using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.DeleteUsageData;

public class DeleteUsageDataEndpoint(
    IMediator mediator
    ) : Endpoint<DeleteUsageDataRequest, EmptyResponse>
{
    public override void Configure()
    {
        Delete("data");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteUsageDataRequest req, CancellationToken cancellationToken)
    {
        await mediator.Send(
            new DeleteUsageDataCommand(
                StartDate: req.StartDate,
                EndDate: req.EndDate
            ),
            cancellationToken
        );
        await Send.NoContentAsync(cancellationToken);
    }
}
