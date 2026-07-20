using H.NotifyIcon.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.State;
using ScreenTimeTracker.Hosts.Desktop.UI.Services;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Versioning;

namespace ScreenTimeTracker.Hosts.Desktop.Platforms;

[SupportedOSPlatform("windows5.1.2600")]
public class TrayService(
    ILogger<TrayService> logger,
    IAppUIManager appUIManager,
    IAppSettingsProvider appSettingsProvider,
    IStringLocalizer<TrayService> localizer,
    IHostApplicationLifetime lifetime) : ITrayService
{
    private Icon? _iconHandle;
    private TrayIconWithContextMenu? _trayIcon;
    private bool _disposed;

    private PopupMenuItem? _openAppDirItem;
    private PopupMenuItem? _openUIBrowserItem;
    private PopupMenuItem? _openUIWindowItem;
    private PopupMenuItem? _exitItem;

    public void Initialize()
    {
        if (_trayIcon is not null) return;

        using var iconStream = typeof(Program).Assembly.GetManifestResourceStream($"AppIcon")
            ?? throw new InvalidOperationException("Icon resource not found.");
        _iconHandle = new Icon(iconStream);

        _openAppDirItem = new PopupMenuItem("打开程序目录", (_, _) => OpenAppDirectory());
        _openUIBrowserItem = new PopupMenuItem("在浏览器打开界面", (_, _) => OpenUIInBrowser());
        _openUIWindowItem = new PopupMenuItem("在窗口打开界面", (_, _) => OpenUIInWindow());
        _exitItem = new PopupMenuItem("退出", (_, _) => ExitApplication());

        _trayIcon = new TrayIconWithContextMenu
        {
            Icon = _iconHandle.Handle,
            ToolTip = "Screen Time Tracker",
            ContextMenu = new PopupMenu
            {
                Items =
                {
                    _openAppDirItem,
                    new PopupMenuSeparator(),
                    _openUIBrowserItem,
                    _openUIWindowItem,
                    new PopupMenuSeparator(),
                    _exitItem,
                },
            }
        };

        ApplyLanguage();
        appSettingsProvider.OnSettingChanged += HandleAppSettingsChanged;

        _trayIcon.MessageWindow.MouseEventReceived += (_, e) =>
        {
            if (e.MouseEvent != MouseEvent.IconLeftMouseUp) return;
            OpenUI();
        };

        _ = InitializeTrayIconWithRetryAsync();
    }

    private void HandleAppSettingsChanged(object? sender, SettingChangedEventArgs e)
    {
        if (e.SettingName == nameof(IAppSettingsProvider.Language))
            ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        if (_trayIcon == null) return;

        var culture = new System.Globalization.CultureInfo(appSettingsProvider.Language);
        System.Globalization.CultureInfo.CurrentUICulture = culture;

        _openAppDirItem?.Text = localizer["OpenAppDirectory"];
        _openUIBrowserItem?.Text = localizer["OpenUIInBrowser"];
        _openUIWindowItem?.Text = localizer["OpenUIInWindow"];
        _exitItem?.Text = localizer["Exit"];
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
