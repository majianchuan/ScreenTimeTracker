using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Features.ActivityLogs.RecordActivity;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.OS;

[SupportedOSPlatform("windows5.0")]
public class WindowsIdleTimeProvider(ILogger<WindowsIdleTimeProvider> logger) : IIdleTimeProvider
{
    private readonly ILogger<WindowsIdleTimeProvider> _logger = logger;

    public Task<TimeSpan> GetSystemIdleTimeAsync()
    {
        LASTINPUTINFO info = new();
        info.cbSize = (uint)Marshal.SizeOf(info);
        if (!PInvoke.GetLastInputInfo(ref info))
        {
            int errorCode = Marshal.GetLastWin32Error();
            _logger.LogWarning("GetLastInputInfo failed with Win32 error code: {ErrorCode}. Assuming active status (TimeSpan.Zero).", errorCode);
            return Task.FromResult(TimeSpan.Zero);
        }

        uint tickCount = PInvoke.GetTickCount();
        uint idleTicks = tickCount - info.dwTime;
        return Task.FromResult(TimeSpan.FromMilliseconds(idleTicks));
    }
}