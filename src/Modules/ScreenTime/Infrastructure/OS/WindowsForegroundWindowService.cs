using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.ScreenTime.Features.ActivityLogs.RecordActivity;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.OS;

[SupportedOSPlatform("windows5.0")]
public class WindowsForegroundWindowService(ILogger<WindowsForegroundWindowService> logger) : IForegroundWindowService
{
    public WindowInfo? GetForegroundWindowInfo()
    {
        HWND hwnd = PInvoke.GetForegroundWindow();
        if (hwnd == HWND.Null)
            return null;
        uint processId;
        unsafe
        {
            _ = PInvoke.GetWindowThreadProcessId(hwnd, &processId);
        }
        if (processId == 0)
            return null;

        Process process = Process.GetProcessById((int)processId);
        string processName = process.ProcessName;
        string? executablePath = null;

        try
        {
            executablePath = process.MainModule?.FileName;
        }
        catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
        {
            logger.LogDebug(ex,
                "Access denied when getting executable path for process {ProcessName}.",
                processName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get executable path for process {ProcessName}.", processName);
        }

        return new WindowInfo(process.ProcessName, executablePath);
    }
}