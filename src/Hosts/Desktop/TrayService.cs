using H.NotifyIcon.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Drawing;

namespace ScreenTimeTracker.Hosts.Desktop;

public class TrayService(
    ILogger<TrayService> logger,
    IAppUIManager appUIManager,
    IHostApplicationLifetime lifetime) : IDisposable
{
    private Icon? _iconHandle;
    private TrayIconWithContextMenu? _trayIcon;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_trayIcon is not null) return;

        using var iconStream = typeof(App).Assembly.GetManifestResourceStream("ScreenTimeTracker.Hosts.Desktop.Resources.Icon.ico")
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

        // 异步非阻塞的托盘重试逻辑，防止卡死主线程
        int maxRetries = 10;
        int delayMilliseconds = 1500;
        for (int i = 0; i < maxRetries; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                _trayIcon.Create();
                break;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("TryCreate failed"))
            {
                logger.LogWarning(ex, "Tray icon creation failed, retrying... ({Retry}/{MaxRetries})", i + 1, maxRetries);
                if (i == maxRetries - 1) throw;
                await Task.Delay(delayMilliseconds, cancellationToken);
            }
        }
    }

    public void Show() => _trayIcon?.Show();

    public void Hide() => _trayIcon?.Hide();

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
        _trayIcon?.Dispose();
        _trayIcon = null;
        _iconHandle?.Dispose();
        _iconHandle = null;
        GC.SuppressFinalize(this);
    }
}
