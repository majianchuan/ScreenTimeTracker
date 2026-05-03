using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ExportData;

public class ExportDataEndpoint(
    IMediator mediator
    ) : EndpointWithoutRequest<ExportDataResponse>
{
    public override void Configure()
    {
        Get("data/export");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        var response = await mediator.Send(
            new ExportDataQuery(),
            cancellationToken
        );
        await Send.OkAsync(response, cancellationToken);
    }
}
