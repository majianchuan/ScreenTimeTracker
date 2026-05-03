using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScreenTimeTracker.Hosts.Desktop;
using ScreenTimeTracker.Modules.ScreenTime;
using ScreenTimeTracker.Modules.AppBehavior;
using Serilog;
using System.IO;
using System.Net;
using System.Windows;
using Mediator;
using ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.GetUserPreferences;
using System.IO.Pipes;
using System.Text;
using System.Security.AccessControl;
using System.Security.Principal;


string MutexName = @"Global\ScreenTimeTrackerDesktopUniqueMutexName";
Mutex? mutex = null;
bool hasMutexOwnership = false;

// 切换工作目录为程序所在目录
Directory.SetCurrentDirectory(AppContext.BaseDirectory);

// 临时日志配置
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File(
        "./startup.log",
        shared: true,
        fileSizeLimitBytes: 1 * 1024 * 1024,
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: 2
    )
    .CreateBootstrapLogger();

Log.Information("Application Started.");

// 全局异常兜底
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    Log.Fatal(e.ExceptionObject as Exception, "AppDomain Unhandled Exception occurred.");
    Log.CloseAndFlush();
};
TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    Log.Fatal(e.Exception, "Unobserved task exception occurred.");
};

try
{
    // 单实例检查，防止重复运行
    try
    {
        var mutexSecurity = new MutexSecurity();

        // 允许所有用户读取/修改 Mutex 状态
        mutexSecurity.AddAccessRule(new MutexAccessRule(
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            MutexRights.FullControl,
            AccessControlType.Allow
        ));
        mutex = MutexAcl.Create(false, MutexName, out _, mutexSecurity);

        try
        {
            hasMutexOwnership = mutex.WaitOne(0);
        }
        catch (AbandonedMutexException)
        {
            // 如果上一个实例崩溃退出，系统会废弃Mutex。
            // 此时当前实例会自动接管 Mutex 的所有权，并抛出此异常。
            Log.Warning("Previous instance crashed. Acquired abandoned mutex.");
            hasMutexOwnership = true;
        }

        if (!hasMutexOwnership)
        {
            Log.Information("Application is already running. Exiting...");
            try
            {
                using var client = new NamedPipeClientStream(".", PipeServerService.PipeName);
                client.Connect(1000);
                byte[] msg = Encoding.UTF8.GetBytes("SHOW");
                client.Write(msg, 0, msg.Length);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to connect to pipe server.");
                MessageBox.Show("程序已经在运行，请查看托盘处", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Log.CloseAndFlush();
            return;
        }
    }
    catch (UnauthorizedAccessException ex)
    {
        Log.Error(ex, "Access denied when creating mutex. Likely another instance is running with higher privileges.");
        MessageBox.Show("已经有一个更高权限的实例在运行，请查看托盘处", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Log.CloseAndFlush();
        return;
    }

    // 构建并启动 WebApplication
    using WebApplication app = BuildWebApplication();
    ConfigureMiddleware(app);

    app.Lifetime.ApplicationStopped.Register(() =>
    {
        Log.Information("Application Stopped.");
    });

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        using var scope = app.Services.CreateScope();
        var serverUrlProvider = scope.ServiceProvider.GetRequiredService<IServerUrlProvider>();
        var serverUrl = serverUrlProvider.GetServerUrl();
        Log.Information("Kestrel server address: {address}", serverUrl);

        // 初始化托盘图标
        var trayService = scope.ServiceProvider.GetRequiredService<TrayService>();
        _ = trayService.InitializeAsync();
        // 非静默启动时打开UI
        _ = OpenUIIfNotSilentStartAsync(app, serverUrl);
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
    MessageBox.Show("程序启动时出错，请检查日志", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    Log.CloseAndFlush();
    Environment.Exit(1);
}
finally
{
    if (mutex is not null)
    {
        if (hasMutexOwnership)
            mutex.ReleaseMutex();
        mutex.Dispose();
    }
    Log.CloseAndFlush();
}

static WebApplication BuildWebApplication()
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
    builder.Services.AddSingleton<IServerUrlProvider, ServerUrlProvider>();
    // WPF
    builder.Services.AddHostedService<WpfHostedService>();
    builder.Services.AddTransient<IViewFactory, ViewFactory>();
    builder.Services.AddSingleton<IAppUIManager, AppUIManager>();
    builder.Services.AddSingleton<IWindowPlacementStore, WindowPlacementStore>();
    builder.Services.Configure<WebViewOptions>(builder.Configuration.GetSection(WebViewOptions.SectionName));
    // 托盘服务
    builder.Services.AddSingleton<TrayService>();
    // 管道服务
    builder.Services.AddHostedService<PipeServerService>();
    // 模块注册
    builder.Services.AddScreenTimeServices(builder.Configuration);
    builder.Services.AddAppBehaviorServices(builder.Configuration);

    return builder.Build();
}

static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
        app.MapOpenApi();  // 文档默认url: /openapi/v1.json
    app.UseStaticFiles();
    app.UseCors(cors =>
    {
        cors.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
    app.UseFastEndpoints(config =>
    {
        config.Endpoints.RoutePrefix = "api";
    });
    app.MapFallbackToFile("index.html"); // SPA回退
}

static async Task OpenUIIfNotSilentStartAsync(WebApplication app, string serverUrl)
{
    using var scope = app.Services.CreateScope();
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    GetUserPreferencesResult userSettings = await mediator.Send(new GetUserPreferencesQuery());
    if (!userSettings.IsSilentStartEnabled)
    {
        var appUIManager = scope.ServiceProvider.GetRequiredService<IAppUIManager>();
        await appUIManager.OpenUIAsync();
    }
}
