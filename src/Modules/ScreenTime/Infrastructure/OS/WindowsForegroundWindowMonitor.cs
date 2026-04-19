using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;
using ScreenTimeTracker.Modules.ScreenTime.Features.Tracking.TrackActiveSession;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Threading;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.WindowsAndMessaging;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.OS;

[SupportedOSPlatform("windows6.0.6000")]
public class WindowsForegroundWindowMonitor : IForegroundWindowMonitor, IDisposable
{
    public event EventHandler<WindowInfo?>? ForegroundWindowChanged;
    private readonly ILogger<WindowsForegroundWindowMonitor> _logger;

    private WINEVENTPROC? _hookProc;
    private uint _threadId;

    public WindowsForegroundWindowMonitor(ILogger<WindowsForegroundWindowMonitor> logger)
    {
        _logger = logger;

        var thread = new Thread(() =>
        {
            try
            {
                _threadId = PInvoke.GetCurrentThreadId();

                _hookProc = OnForegroundChanged;
                using UnhookWinEventSafeHandle hookHandle = PInvoke.SetWinEventHook(
                    PInvoke.EVENT_SYSTEM_FOREGROUND,
                    PInvoke.EVENT_SYSTEM_FOREGROUND,
                    null, _hookProc, 0, 0,
                    PInvoke.WINEVENT_OUTOFCONTEXT);

                if (hookHandle.IsInvalid)
                    throw new Win32Exception(Marshal.GetLastWin32Error(), "Failed to set WinEvent hook.");

                while (true)
                {
                    BOOL ret = PInvoke.GetMessage(out MSG msg, HWND.Null, 0, 0);
                    int value = ret;
                    if (value == -1)
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "GetMessage returned -1 (Error).");
                    if (value == 0)
                        break;
                    PInvoke.TranslateMessage(in msg);
                    PInvoke.DispatchMessage(in msg);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "ForegroundWindowMonitor thread crashed.");
            }
        })
        {
            IsBackground = true,
            Name = "ForegroundWindowMonitorThread"
        };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
    }

    public WindowInfo? GetForegroundWindow()
    {
        HWND hwnd = PInvoke.GetForegroundWindow();
        return GetWindowInfo(hwnd);
    }

    private void OnForegroundChanged(
        HWINEVENTHOOK hWinEventHook, uint @event,
        HWND hwnd, int idObject, int idChild,
        uint idEventThread, uint dwmsEventTime)
    {
        if (@event != PInvoke.EVENT_SYSTEM_FOREGROUND)
            return;
        ForegroundWindowChanged?.Invoke(this, GetWindowInfo(hwnd));
    }

    private WindowInfo? GetWindowInfo(HWND hwnd)
    {
        if (hwnd == HWND.Null)
            return null;

        uint processId;
        unsafe
        {
            _ = PInvoke.GetWindowThreadProcessId(hwnd, &processId);
        }
        if (processId == 0)
            return null;

        // 处理 UWP 应用 (ApplicationFrameHost)
        if (IsApplicationFrameHost(processId, hwnd))
        {
            processId = GetRealProcessIdFromFrameHost(hwnd, processId) ?? processId;
        }

        string? executablePath = GetProcessPath(processId);
        string? processName = executablePath is not null ? Path.GetFileNameWithoutExtension(executablePath) : null;
        if (processName is null)
        {
            using Process process = Process.GetProcessById((int)processId);
            processName = process.ProcessName;
        }

        return new WindowInfo(processName, executablePath);
    }

    private string? GetProcessPath(uint processId)
    {
        HANDLE rawHandle = PInvoke.OpenProcess(
            PROCESS_ACCESS_RIGHTS.PROCESS_QUERY_LIMITED_INFORMATION,
            false, processId);
        if (rawHandle == HANDLE.Null)
            return null;

        using var hProcess = new SafeProcessHandle(rawHandle, ownsHandle: true);

        Span<char> buffer = stackalloc char[1024];
        uint size = (uint)buffer.Length;

        if (PInvoke.QueryFullProcessImageName(
            hProcess,
            PROCESS_NAME_FORMAT.PROCESS_NAME_WIN32,
            buffer, ref size))
            return new string(buffer[..(int)size]);

        return null;
    }

    private static bool IsApplicationFrameHost(uint pid, HWND hwnd)
    {
        Span<char> className = stackalloc char[256];
        unsafe
        {
            fixed (char* pClassName = className)
            {
                int length = PInvoke.GetClassName(hwnd, pClassName, className.Length);
                return className[..length].SequenceEqual("ApplicationFrameWindow");
            }
        }
    }

    private static uint? GetRealProcessIdFromFrameHost(HWND hwnd, uint frameHostPid)
    {
        uint actualPid = 0;
        PInvoke.EnumChildWindows(hwnd, (childHwnd, lparam) =>
        {
            uint childPid;
            unsafe
            {
                _ = PInvoke.GetWindowThreadProcessId(childHwnd, &childPid);
            }
            if (childPid != 0 && childPid != frameHostPid)
            {
                actualPid = childPid;
                return false;
            }
            return true;
        }, 0);

        return actualPid != 0 ? actualPid : null;
    }

    public void Dispose()
    {
        if (_threadId != 0)
            PInvoke.PostThreadMessage(_threadId, PInvoke.WM_QUIT, 0, 0);
        GC.SuppressFinalize(this);
    }
}