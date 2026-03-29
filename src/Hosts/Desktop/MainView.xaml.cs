using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Web.WebView2.Core;
using ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;
using System.ComponentModel;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

public partial class MainView : Window
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IOptions<WebViewOptions> _webViewOptions;
    private readonly IWindowPlacementStore _windowLayoutStore;
    private readonly TaskCompletionSource _webViewReady = new();
    private bool _isClosingConfirmed = false;

    public MainView(IServiceScopeFactory scopeFactory, IOptions<WebViewOptions> webViewOptions, IWindowPlacementStore windowLayoutStore)
    {
        _scopeFactory = scopeFactory;
        _webViewOptions = webViewOptions;
        _windowLayoutStore = windowLayoutStore;
        InitializeComponent();

        MainWebView.CoreWebView2InitializationCompleted += (s, e) =>
        {
            if (e.IsSuccess)
                MainWebView.CoreWebView2.NewWindowRequested += MainWebView_CoreWebView2_NewWindowRequested;
            else
                MessageBox.Show($"Failed to initialize WebView2: {e.InitializationException?.Message}");
        };
    }

    public async void LoadUrl(string url)
    {
        await _webViewReady.Task;
        MainWebView.Source = new Uri(url);
    }

    protected override async void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        // 恢复窗口位置和大小
        RestoreWindowPlacement();

        // 初始化 webview2 环境
        var env = await CoreWebView2Environment.CreateAsync(
            userDataFolder: _webViewOptions.Value.DataDirectoryPath
        );
        await MainWebView.EnsureCoreWebView2Async(env);
        _webViewReady.TrySetResult();
    }

    protected override async void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        // 保存窗口位置和大小
        SaveWindowPlacement();

        if (_isClosingConfirmed)
            return; // 走第二次关闭流程，直接放行

        // 先阻止关闭，等异步结果
        e.Cancel = true;

        // 是否销毁窗口
        using var scope = _scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var userSettings = await mediator.Send(new GetUserSettingsQuery());
        if (userSettings.WindowDestroyOnClose)
        {
            _isClosingConfirmed = true;
            _ = Dispatcher.BeginInvoke(Close); // 触发第二次 Closing，这次放行
        }
        else
            Hide();
    }

    private void EnsureWindowIsVisible()
    {
        if (Left > SystemParameters.VirtualScreenWidth ||
            Top > SystemParameters.VirtualScreenHeight ||
            Left + Width < 0 ||
            Top + Height < 0)
        {
            // 如果跑到可视区域外，重置到屏幕中间
            Left = (SystemParameters.VirtualScreenWidth - Width) / 2;
            Top = (SystemParameters.VirtualScreenHeight - Height) / 2;
        }
    }

    private void RestoreWindowPlacement()
    {
        try
        {
            var settings = _windowLayoutStore.Load();
            if (!double.IsNaN(settings.Left) && !double.IsNaN(settings.Top))
            {
                Left = settings.Left;
                Top = settings.Top;
                Width = settings.Width;
                Height = settings.Height;
            }
            if (settings.IsMaximized)
                WindowState = WindowState.Maximized;
            EnsureWindowIsVisible();
        }
        catch { }
    }

    private void SaveWindowPlacement()
    {
        // 保存窗口位置和大小
        WindowPlacement placement = new();
        if (WindowState == WindowState.Maximized || WindowState == WindowState.Minimized)
        {
            placement.Left = RestoreBounds.Left;
            placement.Top = RestoreBounds.Top;
            placement.Width = RestoreBounds.Width;
            placement.Height = RestoreBounds.Height;
            placement.IsMaximized = WindowState == WindowState.Maximized;
        }
        else
        {
            placement.Left = Left;
            placement.Top = Top;
            placement.Width = Width;
            placement.Height = Height;
            placement.IsMaximized = false;
        }
        _windowLayoutStore.Save(placement);
    }

    private void MainWebView_CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        // 获取请求的 URL
        string url = e.Uri;
        // 阻止 WebView2 自己打开新窗口
        e.Handled = true;
        // 在系统默认浏览器中打开
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}

public class WindowPlacement
{
    public double Left { get; set; } = double.NaN;
    public double Top { get; set; } = double.NaN;
    public double Width { get; set; } = 1000;
    public double Height { get; set; } = 800;
    public bool IsMaximized { get; set; } = false;
}

public interface IWindowPlacementStore
{
    WindowPlacement Load();
    void Save(WindowPlacement settings);
}
