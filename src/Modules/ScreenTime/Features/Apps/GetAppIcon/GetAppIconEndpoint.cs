using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.StaticFiles;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.Apps.GetAppIcon;

public class GetAppIconEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppIconRequest, EmptyResponse>
{
    public override void Configure()
    {
        Get("apps/{id}/icon");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppIconRequest req, CancellationToken cancellationToken)
    {
        string? iconPath = await mediator.Send(
            new GetAppIconPathQuery(req.Id),
            cancellationToken
        );

        if (string.IsNullOrEmpty(iconPath) || !File.Exists(iconPath))
        {
            await Send.NotFoundAsync(cancellationToken);
            return;
        }

        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(iconPath, out var contentType))
        {
            contentType = "image/png"; // 默认 png 图片
        }

        await Send.FileAsync(new FileInfo(iconPath), contentType, cancellation: cancellationToken);
    }
}
