using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Web.WebView2.Core;
using ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

public partial class MainView : Window
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<WebViewOptions> _webViewOptions;
    private readonly WindowPlacementStore _windowLayoutStore;
    private string? _pendingUrl;

    public MainView(IServiceProvider serviceProvider, IOptions<WebViewOptions> webViewOptions, WindowPlacementStore windowLayoutStore)
    {
        _serviceProvider = serviceProvider;
        _webViewOptions = webViewOptions;
        _windowLayoutStore = windowLayoutStore;
        InitializeComponent();

        // 初始化 webview2
        SourceInitialized += MainView_SourceInitialized_InitializeWebView2;
        SourceInitialized += MainView_SourceInitialized_LoadWindowPlacement;

        MainWebView.CoreWebView2InitializationCompleted += async (s, e) =>
        {
            if (e.IsSuccess)
            {
                MainWebView.CoreWebView2.NewWindowRequested += MainWebView_CoreWebView2_NewWindowRequested;
            }
            else
            {
                MessageBox.Show(e.InitializationException?.Message);
            }
        };

        Closing += MainWebView_Closing;
    }

    public void LoadUrl(string url)
    {
        if (MainWebView.CoreWebView2 == null)
        {
            _pendingUrl = url;
            return;
        }
        MainWebView.Source = new Uri(url);
    }


    private async void MainView_SourceInitialized_InitializeWebView2(object? sender, EventArgs e)
    {
        var env = await CoreWebView2Environment.CreateAsync(
            userDataFolder: _webViewOptions.Value.DataDirectoryPath
        );
        await MainWebView.EnsureCoreWebView2Async(env);

        // 初始化完成后再加载 URL
        if (!string.IsNullOrEmpty(_pendingUrl))
        {
            MainWebView.Source = new Uri(_pendingUrl);
            _pendingUrl = null;
        }
    }

    private async void MainView_SourceInitialized_LoadWindowPlacement(object? sender, EventArgs e)
    {
        try
        {
            WindowPlacement settings = _windowLayoutStore.Load();

            if (!double.IsNaN(settings.Left) && !double.IsNaN(settings.Top))
            {
                Left = settings.Left;
                Top = settings.Top;
                Width = settings.Width;
                Height = settings.Height;
            }

            if (settings.IsMaximized)
                WindowState = WindowState.Maximized;

            // 如果用户上次在副屏关闭了程序，拔掉副屏后，坐标可能会跑到屏幕外！
            // 以下这行代码可以确保窗口如果跑到屏幕外面了，能自动被拉回到主屏幕可见区域。
            EnsureWindowIsVisible();
        }
        catch
        {
            // 忽略异常，使用 XAML 默认的大小和位置
        }
    }

    private void EnsureWindowIsVisible()
    {
        if (Left > SystemParameters.VirtualScreenWidth ||
            Top > SystemParameters.VirtualScreenHeight ||
            Left + Width < 0 ||
            Top + Height < 0)
        {
            // 如果跑到可视区域外，重置到屏幕中间
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }

    private async void MainWebView_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        using var scope = _serviceProvider.CreateScope();
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

        // 是否销毁窗口
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var userSettings = await mediator.Send(new GetUserSettingsQuery());
        if (userSettings.WindowDestroyOnClose)
            return;

        e.Cancel = true;
        Hide();
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

public class WindowPlacementStore(ILogger<WindowPlacementStore> logger)
{
    private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WindowSettings.json");
    private static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    public WindowPlacement Load()
    {
        if (File.Exists(ConfigPath))
        {
            try
            {
                string json = File.ReadAllText(ConfigPath);
                // 反序列化，如果为空则返回默认的新对象
                return JsonSerializer.Deserialize<WindowPlacement>(json) ?? new WindowPlacement();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "加载窗口布局配置失败");
                return new WindowPlacement();
            }
        }
        return new WindowPlacement();
    }

    public void Save(WindowPlacement settings)
    {
        try
        {
            string json = JsonSerializer.Serialize(settings, jsonSerializerOptions);
            File.WriteAllText(ConfigPath, json);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "保存窗口布局配置失败");
        }
    }
}