using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.StaticFiles;
using ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategory;

namespace ScreenTimeTracker.Modules.ScreenTime.Features.AppCategories.GetAppCategoryIcon;

public class GetAppCategoryIconEndpoint(
    IMediator mediator
    ) : Endpoint<GetAppCategoryIconRequest, EmptyResponse>
{
    public override void Configure()
    {
        Get("app-categories/{appCategoryId}/icon");
        Group<ScreenTimeGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetAppCategoryIconRequest req, CancellationToken cancellationToken)
    {
        var appCategory = await mediator.Send(
            new GetAppCategoryQuery(req.AppCategoryId),
            cancellationToken
        );
        var iconPath = appCategory?.IconPath;

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
