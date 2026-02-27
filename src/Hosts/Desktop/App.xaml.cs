using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScreenTimeTracker.Modules.ScreenTime;
using ScreenTimeTracker.Modules.Shell;
using ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;
using Serilog;
using System.IO;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static Mutex? _mutex;
    private WebApplication? _app;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 防止重复运行
        _mutex = new(true, "ScreenTimeTrackerDesktopUniqueMutexName", out bool createdNew);
        if (!createdNew)
        {
            MessageBox.Show("程序已经运行，请查看托盘处图标", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            Current.Shutdown();
            return;
        }
        // 切换工作目录为程序所在目录
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        var builder = WebApplication.CreateBuilder();

        ConfigureCommonServices(builder.Services, builder.Configuration);
        ConfigureWebApiServices(builder.Services, builder.Configuration);
        ConfigureWpfServices(builder.Services, builder.Configuration);
        builder.Services.AddScreenTimeServices(builder.Configuration);
        builder.Services.AddShellServices(builder.Configuration);

        _app = builder.Build();

        if (_app.Environment.IsDevelopment())
            _app.MapOpenApi();  // 文档默认url: /openapi/v1.json
        _app.UseStaticFiles(); // 托管前端静态文件
        _app.UseCors(builder => builder // 允许所有来源、方法、头
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
        _app.UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
        });
        _app.MapFallbackToFile("index.html"); // SPA回退

        try
        {
            await _app.StartAsync();
        }
        catch (IOException ex)
        {
            var logger = _app.Services.GetRequiredService<ILogger<App>>();
            logger.LogError(ex, "Web Application start failed.");
            MessageBox.Show("端口已被占用，请检查是否有其他实例正在运行", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
            return;
        }

        _app.Services.GetRequiredService<NotifyIconView>();

        OpenMainViewIfNeed();
    }

    private async void OpenMainViewIfNeed()
    {
        if (_app is null)
            return;
        // 如果不是静默启动，就打开主窗口
        using var scope = _app.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var userSettings = await mediator.Send(new GetUserSettingsQuery());
        if (userSettings.SilentStart)
            return;
        var options = _app.Services.GetRequiredService<IOptions<NotifyIconOptions>>();
        var mainView = _app.Services.GetRequiredService<MainView>();
        mainView.LoadUrl(options.Value.UIUrl);
        mainView.Show();
        mainView.Activate();
        Current.MainWindow = mainView;
    }

    private static void ConfigureCommonServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSerilog((services, loggerConfig) =>
        {
            loggerConfig.ReadFrom.Configuration(configuration);
        });
        services.AddMediator(options =>
        {
            options.Namespace = "ScreenTimeTracker.Mediator";
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        services.AddSingleton<TimeProvider>(TimeProvider.System);
    }

    private static void ConfigureWebApiServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi();
        services.AddFastEndpoints();
        services.AddCors();
    }

    private static void ConfigureWpfServices(IServiceCollection services, IConfiguration configuration)
    {
        // 注册 Views
        services.AddTransient<MainView>();
        services.AddSingleton<NotifyIconView>();

        // 注册 ViewModels
        services.AddSingleton<NotifyIconViewModel>();

        // 注册配置
        services.Configure<NotifyIconOptions>(configuration.GetSection(NotifyIconOptions.SectionName));
        services.Configure<WebViewOptions>(configuration.GetSection(WebViewOptions.SectionName));
    }
}

