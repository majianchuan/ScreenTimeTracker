using H.NotifyIcon.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.State;
using ScreenTimeTracker.Hosts.Desktop.UI.Services;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Versioning;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

namespace ScreenTimeTracker.Hosts.Desktop.Platforms;

[SupportedOSPlatform("windows5.1.2600")]
public class TrayService : ITrayService, IDisposable
{
    private readonly ILogger<TrayService> _logger;
    private readonly IAppUIManager _appUIManager;
    private readonly IAppSettingsProvider _appSettingsProvider;
    private readonly IStringLocalizer<TrayService> _localizer;
    private readonly IHostApplicationLifetime _lifetime;

    private readonly PopupMenuItem _openAppDirItem;
    private readonly PopupMenuItem _openUIBrowserItem;
    private readonly PopupMenuItem _openUIWindowItem;
    private readonly PopupMenuItem _exitItem;

    private readonly Icon? _iconHandle;
    private nint _fallbackIconHandle = nint.Zero;
    private readonly TrayIconWithContextMenu _trayIcon;
    private bool _isCreated = false;
    private bool _shouldBeVisible = false;
    private bool _isVisible = false;
    private bool _disposed;

    public TrayService(
        ILogger<TrayService> logger,
        IAppUIManager appUIManager,
        IAppSettingsProvider appSettingsProvider,
        IStringLocalizer<TrayService> localizer,
        IHostApplicationLifetime lifetime)
    {
        _logger = logger;
        _appUIManager = appUIManager;
        _appSettingsProvider = appSettingsProvider;
        _localizer = localizer;
        _lifetime = lifetime;

        using var iconStream = typeof(Program).Assembly.GetManifestResourceStream("AppIcon");
        if (iconStream is not null)
            _iconHandle = new Icon(iconStream);
        else
        {
            logger.LogWarning("Embedded resource 'AppIcon' not found. Creating a fallback solid color icon.");
            using var bitmap = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bitmap);
            g.Clear(Color.RoyalBlue);
            using var pen = new Pen(Color.White, 2);
            g.DrawRectangle(pen, 2, 2, 28, 28);
            _fallbackIconHandle = bitmap.GetHicon();
            _iconHandle = null;
        }

        _openAppDirItem = new PopupMenuItem(string.Empty, (_, _) => OpenAppDirectory());
        _openUIBrowserItem = new PopupMenuItem(string.Empty, (_, _) => OpenUIInBrowser());
        _openUIWindowItem = new PopupMenuItem(string.Empty, (_, _) => OpenUIInWindow());
        _exitItem = new PopupMenuItem(string.Empty, (_, _) => ExitApplication());

        _trayIcon = new TrayIconWithContextMenu
        {
            Icon = _iconHandle is null ? _fallbackIconHandle : _iconHandle.Handle,
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

        _trayIcon.MessageWindow.TaskbarCreated += OnTaskbarCreated;

        _appSettingsProvider.OnSettingChanged += HandleAppSettingsChanged;

        _trayIcon.MessageWindow.MouseEventReceived += (_, e) =>
        {
            if (e.MouseEvent != MouseEvent.IconLeftMouseUp) return;
            OpenUI();
        };

    }

    public void Show()
    {
        if (_shouldBeVisible)
            return;
        _shouldBeVisible = true;
        if (_isVisible)
            return;

        if (!_isCreated)
        {
            try
            {
                _trayIcon.Create();
                _isCreated = true;
                _isVisible = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create tray icon. Waiting for taskbar to be created/recreated.");
            }
        }
        else
        {
            _trayIcon.Show();
            _isVisible = true;
        }
    }

    public void Hide()
    {
        if (!_shouldBeVisible)
            return;

        _shouldBeVisible = false;
        if (_isCreated && _isVisible)
        {
            _trayIcon.Hide();
            _isVisible = false;
        }
    }

    private void OnTaskbarCreated(object? sender, EventArgs e)
    {
        _logger.LogInformation("Taskbar created/recreated.");

        if (_isCreated)
        {
            try
            {
                _trayIcon.TryRemove();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to remove old tray icon registration.");
            }
            _isCreated = false;
        }
        if (_shouldBeVisible)
        {
            _trayIcon.Create();
            _isCreated = true;
            _isVisible = true;
        }
        else
            _isVisible = false;
    }

    private void HandleAppSettingsChanged(object? sender, SettingChangedEventArgs e)
    {
        if (e.SettingName == nameof(IAppSettingsProvider.Language))
            ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        var culture = new System.Globalization.CultureInfo(_appSettingsProvider.Language);
        System.Globalization.CultureInfo.CurrentUICulture = culture;

        _openAppDirItem.Text = _localizer["OpenAppDirectory"];
        _openUIBrowserItem.Text = _localizer["OpenUIInBrowser"];
        _openUIWindowItem.Text = _localizer["OpenUIInWindow"];
        _exitItem.Text = _localizer["Exit"];
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
        _lifetime.StopApplication();
    }

    private async void OpenUI()
    {
        try
        {
            await _appUIManager.OpenUIAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open UI.");
        }
    }

    private async void OpenUIInBrowser()
    {
        try
        {
            await _appUIManager.OpenUIInBrowserAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open UI in browser.");
        }
    }

    private async void OpenUIInWindow()
    {
        try
        {
            await _appUIManager.OpenUIInWindowAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to open UI in window.");
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
            _appSettingsProvider.OnSettingChanged -= HandleAppSettingsChanged;
            _trayIcon.Dispose();
            _iconHandle?.Dispose();
        }

        if (_fallbackIconHandle != nint.Zero)
        {
            PInvoke.DestroyIcon(new HICON(_fallbackIconHandle));
            _fallbackIconHandle = nint.Zero;
        }

        _disposed = true;
    }
}
