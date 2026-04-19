using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using ScreenTimeTracker.Modules.ScreenTime.Features.Tracking.TrackActiveSession;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.OS;

[SupportedOSPlatform("windows")]
public class WindowsExecutableMetadataProvider : IExecutableMetadataProvider
{
    public Task<ExecutableMetadata> GetMetadataAsync(string exePath)
    {
        string? description = FileVersionInfo.GetVersionInfo(exePath)?.FileDescription;
        using Icon? icon = Icon.ExtractAssociatedIcon(exePath);
        if (icon is null)
            return Task.FromResult(new ExecutableMetadata(description, null, null));
        using Bitmap? bmp = icon.ToBitmap();
        if (bmp is null)
            return Task.FromResult(new ExecutableMetadata(description, null, null));

        // 不能用 using，交给调用者使用和释放
        MemoryStream ms = new();
        bmp.Save(ms, ImageFormat.Png); // 写完数据后会导致 ms.Position == ms.Length
        ms.Position = 0; // 重置 Position 到开头，方便调用者读取数据

        return Task.FromResult(new ExecutableMetadata(
            description,
            ms,
            "png"
        ));
    }
}