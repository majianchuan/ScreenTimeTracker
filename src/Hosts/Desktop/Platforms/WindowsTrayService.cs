using H.NotifyIcon.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Hosts.Desktop.UI.Services;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Versioning;

namespace ScreenTimeTracker.Hosts.Desktop.Platforms;

[SupportedOSPlatform("windows5.1.2600")]
public class TrayService(
    ILogger<TrayService> logger,
    IAppUIManager appUIManager,
    IHostApplicationLifetime lifetime) : ITrayService
{
    private Icon? _iconHandle;
    private TrayIconWithContextMenu? _trayIcon;
    private bool _disposed;


    public void Initialize()
    {
        if (_trayIcon is not null) return;

        using var iconStream = typeof(Program).Assembly.GetManifestResourceStream($"AppIcon")
            ?? throw new InvalidOperationException("Icon resource not found.");
        _iconHandle = new Icon(iconStream);

        _trayIcon = new TrayIconWithContextMenu
        {
            Icon = _iconHandle.Handle,
            ToolTip = "Screen Time Tracker",
            ContextMenu = new PopupMenu
            {
                Items =
                {
                    new PopupMenuItem("打开程序目录", (_, _) => OpenAppDirectory()),
                    new PopupMenuSeparator(),
                    new PopupMenuItem("在浏览器打开界面", (_, _) => OpenUIInBrowser()),
                    new PopupMenuItem("在窗口打开界面", (_, _) => OpenUIInWindow()),
                    new PopupMenuSeparator(),
                    new PopupMenuItem("退出", (_, _) => ExitApplication()),
                }
            }
        };

        _trayIcon.MessageWindow.MouseEventReceived += (_, e) =>
        {
            if (e.MouseEvent != MouseEvent.IconLeftMouseUp) return;
            OpenUI();
        };

        _ = InitializeTrayIconWithRetryAsync();
    }

    private async Task InitializeTrayIconWithRetryAsync()
    {
        int maxRetries = 10;
        int delayMilliseconds = 1000;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                _trayIcon!.Create();
                break;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("TryCreate failed"))
            {
                if (i == maxRetries - 1) throw;
                Thread.Sleep(delayMilliseconds);
            }
        }
    }

    private static void OpenAppDirectory()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = AppContext.BaseDirectory,
            UseShellExecute = true
        });
    }

    private void ExitApplication()
    {
        lifetime.StopApplication();
    }

    private async void OpenUI()
    {
        try
        {
            await appUIManager.OpenUIAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to open UI.");
        }
    }

    private async void OpenUIInBrowser()
    {
        try
        {
            await appUIManager.OpenUIInBrowserAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to open UI in browser.");
        }
    }

    private async void OpenUIInWindow()
    {
        try
        {
            await appUIManager.OpenUIInWindowAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to open UI in window.");
        }
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _trayIcon?.Dispose();
            _trayIcon = null;
            _iconHandle?.Dispose();
            _iconHandle = null;
        }

        _disposed = true;
    }
}
