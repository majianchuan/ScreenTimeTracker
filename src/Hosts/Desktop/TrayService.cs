using H.NotifyIcon.Core;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.GetUserPreferences;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

public class TrayService(
    ILogger<TrayService> logger,
    IServiceScopeFactory scopeFactory,
    ViewFactory viewFactory,
    IHostApplicationLifetime lifetime) : IDisposable
{
    private string _uiUrl = string.Empty;
    private Icon? _iconHandle;
    private TrayIconWithContextMenu? _trayIcon;

    public async Task InitializeAsync(string uiUrl, CancellationToken cancellationToken = default)
    {
        if (_trayIcon is not null) return;

        _uiUrl = string.IsNullOrWhiteSpace(uiUrl)
            ? throw new ArgumentException("UI URL cannot be null or empty.", nameof(uiUrl))
            : uiUrl;

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
                    new PopupMenuItem("退出", (_, _) => lifetime.StopApplication()),
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


    private static void OpenAppDirectory()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = AppContext.BaseDirectory,
            UseShellExecute = true
        });
    }

    private void OpenUIInBrowser()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = _uiUrl,
            UseShellExecute = true
        });
    }

    public void Show() => _trayIcon?.Show();

    public void Hide() => _trayIcon?.Hide();

    private async void OpenUI()
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var settings = await scope.ServiceProvider.GetRequiredService<IMediator>().Send(new GetUserPreferencesQuery());

            if (settings.UIOpenMode == UIOpenModeDto.Browser)
                OpenUIInBrowser();
            else
                OpenUIInWindow();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to open UI.");
        }
    }

    private void OpenUIInWindow()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (Application.Current.MainWindow is not MainView mainView)
            {
                mainView = viewFactory.Create<MainView>();
                mainView.LoadUrl(_uiUrl);
                Application.Current.MainWindow = mainView;
            }
            else if (mainView.WindowState == WindowState.Minimized)
            {
                mainView.WindowState = WindowState.Normal;
            }

            mainView.Show();
            mainView.Activate();

            // 解决 Windows 偶尔不置顶的问题
            if (!mainView.Topmost)
            {
                mainView.Topmost = true;
                mainView.Topmost = false;
            }
        });
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
