using H.NotifyIcon.Core;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;
using System.Diagnostics;
using System.Drawing;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

public sealed class TrayService(
    ILogger<TrayService> logger,
    IServiceScopeFactory scopeFactory,
    IViewFactory viewFactory,
    IHostApplicationLifetime hostApplicationLifetime) : IDisposable
{
    public string UIUrl { get; set; } = string.Empty;
    private Icon? _iconHandle;
    private TrayIcon? _trayIcon;

    public void CreateTrayIcon()
    {
        if (_trayIcon is not null)
            return;

        using var iconStream = typeof(App).Assembly.GetManifestResourceStream("ScreenTimeTracker.Hosts.Desktop.Resources.Icon.ico");
        if (iconStream is null)
        {
            logger.LogError("ScreenTimeTracker.Hosts.Desktop.Resources.Icon.ico not found.");
            throw new InvalidOperationException("Icon.ico not found.");
        }
        _iconHandle = new Icon(iconStream);
        _trayIcon = new TrayIconWithContextMenu
        {
            Icon = _iconHandle.Handle,
            ToolTip = "Screen Time Tracker",
            Visibility = IconVisibility.Hidden, // 创建时不要显示
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
        _trayIcon.MessageWindow.MouseEventReceived += async (_, e) =>
        {
            if (e.MouseEvent == MouseEvent.IconLeftMouseUp)
                await OpenUIAsync();
        };
        _trayIcon.Create();
    }

    public void ShowTrayIcon()
    {
        if (_trayIcon is null)
            CreateTrayIcon();
        _trayIcon?.Show();
    }

    public void HideTrayIcon()
    {
        _trayIcon?.Hide();
    }

    private static void OpenAppDirectory()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = AppContext.BaseDirectory,
            UseShellExecute = true
        });
    }

    private async Task OpenUIAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        GetUserSettingsResult userSettings = await mediator.Send(new GetUserSettingsQuery());
        if (userSettings.UIOpenMode == UIOpenModeDto.Browser)
            OpenUIInBrowser();
        else
            OpenUIInWindow();
    }

    private void OpenUIInBrowser()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = UIUrl,
            UseShellExecute = true
        });
    }

    private void OpenUIInWindow()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (Application.Current.MainWindow is not MainView mainView)
            {
                mainView = viewFactory.Create<MainView>();
                mainView.LoadUrl(UIUrl);
                Application.Current.MainWindow = mainView;
            }
            else if (mainView.WindowState == WindowState.Minimized)
            {
                mainView.WindowState = WindowState.Normal;
            }
            mainView.Show();
            mainView.Activate();
        });
    }

    private void ExitApplication()
    {
        hostApplicationLifetime.StopApplication();
    }

    public void Dispose()
    {
        _trayIcon?.Dispose();
        _iconHandle?.Dispose();
        GC.SuppressFinalize(this);
    }
}