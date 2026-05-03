using FastEndpoints;
using Mediator;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.DataManagement.ImportData;

public class ImportDataEndpoint(
    IMediator mediator
    ) : EndpointWithoutRequest<ImportDataResponse>
{
    public override void Configure()
    {
        Post("data/import");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(HttpContext.Request.Body);
        var rawJson = await reader.ReadToEndAsync(cancellationToken);

        try
        {
            var response = await mediator.Send(
                new ImportDataCommand(rawJson),
                cancellationToken
            );
            await Send.OkAsync(response, cancellationToken);
        }
        catch (NotSupportedException)
        {
            await Send.ErrorsAsync(statusCode: 422, cancellationToken);
        }
    }
}
