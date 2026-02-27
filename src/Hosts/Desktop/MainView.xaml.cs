using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Web.WebView2.Core;
using ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

public partial class MainView : Window
{
    private readonly IServiceProvider _serviceProvider;
    private string? _pendingUrl;

    public MainView(IOptions<WebViewOptions> webViewOptions, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();

        // 初始化 webview2
        SourceInitialized += async (s, e) =>
        {
            var env = await CoreWebView2Environment.CreateAsync(
                userDataFolder: webViewOptions.Value.DataDirectoryPath
            );
            await MainWebView.EnsureCoreWebView2Async(env);

            // 初始化完成后再加载 URL
            if (!string.IsNullOrEmpty(_pendingUrl))
            {
                MainWebView.Source = new Uri(_pendingUrl);
                _pendingUrl = null;
            }
        };

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

    private async void MainWebView_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        using var scope = _serviceProvider.CreateScope();
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