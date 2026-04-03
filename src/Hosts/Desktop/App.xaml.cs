using FastEndpoints;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScreenTimeTracker.Modules.ScreenTime;
using ScreenTimeTracker.Modules.Shell;
using ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;
using Serilog;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private const string MutexName = @"Local\ScreenTimeTrackerDesktopUniqueMutexName";
    private Mutex? _mutex;
    private bool _isMutexOwner;
    private WebApplication? _app;

    public App()
    {
        // 切换工作目录为程序所在目录
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);

        // 临时日志配置
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                "./startup.log",
                shared: true,
                fileSizeLimitBytes: 1 * 1024 * 1024,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: 2
            )
            .CreateBootstrapLogger();

        Log.Information("App Started.");

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            Log.Fatal(e.ExceptionObject as Exception, "AppDomain Unhandled Exception occurred.");
            Log.CloseAndFlush();
        };
        TaskScheduler.UnobservedTaskException += (sender, e) =>
        {
            Log.Fatal(e.Exception, "Unobserved task exception occurred.");
        };
        DispatcherUnhandledException += (sender, e) =>
        {
            Log.Fatal(e.Exception, "WPF UI thread terminated unexpectedly.");
            Log.CloseAndFlush();
            MessageBox.Show("程序发生未知错误，即将退出", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        };
        SessionEnding += (s, e) =>
        {
            Log.Information("System shutting down, stopping...");
            var lifetime = _app?.Services.GetRequiredService<IHostApplicationLifetime>();
            lifetime?.StopApplication();
        };
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);

        if (_isMutexOwner)
            _mutex?.ReleaseMutex();
        _mutex?.Dispose();

        Log.Information("App Exited.");
        Log.CloseAndFlush();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            // 防止重复运行
            _mutex = new(true, MutexName, out bool createdNew);
            if (!createdNew)
            {
                _isMutexOwner = false;
                Log.Information("App is already running. Exiting...");
                MessageBox.Show("程序已经在运行，请查看托盘处", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
                return;
            }
            _isMutexOwner = true;
        }
        catch (UnauthorizedAccessException ex)
        {
            _isMutexOwner = false;
            Log.Error(ex, "UnauthorizedAccessException occurred.");
            MessageBox.Show("已经有一个更高权限的实例在运行，请查看托盘处", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
            return;
        }

        try
        {
            _app = BuildWebApplication();
            ConfigureMiddleware(_app);

            await _app.StartAsync();

            _ = Task.Run(async () =>
            {
                // 等待 WebApplication 停止，然后退出 WPF 应用
                await _app.WaitForShutdownAsync();
                Log.Information("Web Application Stopped.");
                await Current.Dispatcher.InvokeAsync(() => Current.Shutdown());
            });

            // 获取后端监听的地址
            var server = _app.Services.GetRequiredService<IServer>();
            string? serverUrl = server.Features.Get<IServerAddressesFeature>()?.Addresses?.FirstOrDefault();
            if (serverUrl is null)
            {
                Log.Error("Kestrel server addresses not found.");
                MessageBox.Show("Kestrel 服务器地址未找到，请检查配置", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
                return;
            }
            Log.Information("Kestrel server address: {address}", serverUrl);

            InitializeTrayIcon(_app, serverUrl);
            await OpenUIIfNotSilentStartAsync(_app, serverUrl);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "App Startup terminated unexpectedly.");
            MessageBox.Show("程序启动时出错，请检查日志", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
            return;
        }
    }

    private static WebApplication BuildWebApplication()
    {
        var builder = WebApplication.CreateBuilder();
        // 系统分配端口
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(IPAddress.Loopback, 0);
        });
        // 日志
        builder.Services.AddSerilog((services, loggerConfig) =>
        {
            loggerConfig.ReadFrom.Configuration(builder.Configuration);
        });
        // 通用服务
        builder.Services.AddMediator(options =>
        {
            options.Namespace = "ScreenTimeTracker.Mediator";
            options.ServiceLifetime = ServiceLifetime.Scoped;
        });
        builder.Services.AddSingleton(TimeProvider.System);
        // Web API
        builder.Services.AddOpenApi();
        builder.Services.AddFastEndpoints();
        builder.Services.AddCors();
        // WPF
        builder.Services.AddTransient<IViewFactory, ViewFactory>();
        builder.Services.AddSingleton<IWindowPlacementStore, WindowPlacementStore>();
        builder.Services.Configure<WebViewOptions>(builder.Configuration.GetSection(WebViewOptions.SectionName));
        // 托盘服务
        builder.Services.AddSingleton<TrayService>();
        // 模块注册
        builder.Services.AddScreenTimeServices(builder.Configuration);
        builder.Services.AddShellServices(builder.Configuration);

        return builder.Build();
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
            app.MapOpenApi();  // 文档默认url: /openapi/v1.json
        app.UseStaticFiles();
        app.UseCors(cors =>
        {
            cors.AllowAnyMethod().AllowAnyHeader();
            if (app.Environment.IsDevelopment())
                cors.AllowAnyOrigin();
        });
        app.UseFastEndpoints(config =>
        {
            config.Endpoints.RoutePrefix = "api";
        });
        app.MapFallbackToFile("index.html"); // SPA回退
    }

    private static void InitializeTrayIcon(WebApplication app, string serverUrl)
    {
        var trayService = app.Services.GetRequiredService<TrayService>();
        trayService.UIUrl = serverUrl;
        trayService.ShowTrayIcon();
    }

    private static async Task OpenUIIfNotSilentStartAsync(WebApplication app, string serverUrl)
    {
        using var scope = app.Services.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        GetUserSettingsResult userSettings = await mediator.Send(new GetUserSettingsQuery());
        if (!userSettings.SilentStart)
        {
            if (userSettings.UIOpenMode == UIOpenModeDto.Browser)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = serverUrl,
                    UseShellExecute = true
                });
            }
            else
            {
                var viewFactory = app.Services.GetRequiredService<IViewFactory>();
                var mainView = viewFactory.Create<MainView>();
                mainView.LoadUrl(serverUrl);
                mainView.Show();
                Current.MainWindow = mainView;
            }
        }
    }
}

