using FastEndpoints;
using FastEndpoints.Swagger;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScreenTimeTracker.Hosts.Desktop.Hosting;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings;
using ScreenTimeTracker.Hosts.Desktop.LocalSettings.Features.AppSettingsManagement.GetAppSettings;
using ScreenTimeTracker.Hosts.Desktop.Platforms;
using ScreenTimeTracker.Hosts.Desktop.UI.Services;
using ScreenTimeTracker.Hosts.Desktop.UI.State;
using ScreenTimeTracker.Modules.ScreenTime;
using Serilog;
using System.Text.Json.Serialization;
using Windows.Win32;
using Windows.Win32.UI.WindowsAndMessaging;

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
    var builder = WebApplication.CreateBuilder();
    // 通用服务
    builder.Services.AddSerilog((services, loggerConfig) =>
    {
        loggerConfig.ReadFrom.Configuration(builder.Configuration);
    });
    builder.Services.AddMediator(options =>
    {
        options.Namespace = "ScreenTimeTracker.Mediator";
        options.ServiceLifetime = ServiceLifetime.Scoped;
    });
    builder.Services.AddSingleton(TimeProvider.System);
    // Web API
    builder.Services.AddFastEndpoints();
    builder.Services.SwaggerDocument();
    builder.Services.AddCors();
    builder.Services.AddSingleton<IServerUrlProvider, ServerUrlProvider>();
    // 桌面
    builder.Services.AddSingleton<IAppUIManager, AppUIManager>();
    builder.Services.AddSingleton<IWindowPlacementStore, WindowPlacementStore>();
    // 平台特异
    if (OperatingSystem.IsWindowsVersionAtLeast(5, 1, 2600))
    {
        builder.Services.AddSingleton<ISingleInstanceLock, WindowsSingleInstanceLock>();
        builder.Services.AddSingleton<IInstanceMessenger, WindowsInstanceMessenger>();
        builder.Services.AddSingleton<ITrayService, TrayService>();
    }
    else
    {
        throw new PlatformNotSupportedException("Only Windows XP RTM or later is supported.");
    }

    // 模块注册
    builder.Services.AddScreenTimeServices(builder.Configuration);
    builder.Services.AddLocalSettingsServices(builder.Configuration);

    using WebApplication app = builder.Build();

    // 单例检测
    var singleInstanceLock = app.Services.GetRequiredService<ISingleInstanceLock>();
    var instanceMessenger = app.Services.GetRequiredService<IInstanceMessenger>();
    if (!singleInstanceLock.TryAcquire())
    {
        Log.Information("Application is already running.");
        if (!await instanceMessenger.SendMessageAsync("Show"))
        {
            Log.Error("Failed to send message to existing instance.");
            ShowErrorDialog("错误！", "程序已经在运行，请查看托盘处");
        }
        Log.CloseAndFlush();
        return;
    }
    instanceMessenger.MessageReceived += (sender, e) =>
    {
        if (e.Message == "Show")
        {
            Log.Information("Received message from existing instance to show UI.");
            var appUIManager = app.Services.GetRequiredService<IAppUIManager>();
            _ = appUIManager.OpenUIAsync();
        }
    };
    await instanceMessenger.StartListeningAsync();

    // 中间件
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
        // 枚举转和字符串相互转换
        config.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
    });
    app.UseSwaggerGen();  // 文档url: /swagger/v1/swagger.json
    app.MapFallbackToFile("index.html"); // SPA回退

    app.Lifetime.ApplicationStopped.Register(() =>
    {
        Log.Information("Application Stopped.");
    });

    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var serverUrlProvider = app.Services.GetRequiredService<IServerUrlProvider>();
        var serverUrl = serverUrlProvider.GetServerUrl();
        Log.Information("Kestrel server address: {address}", serverUrl);

        // 初始化托盘图标
        var trayService = app.Services.GetRequiredService<ITrayService>();
        trayService.Initialize();
        // 非静默启动时打开UI
        _ = OpenUIIfNotSilentStartAsync(app, serverUrl);
    });

    try
    {
        app.Run();
    }
    catch (IOException ex) when (ex.InnerException is AddressInUseException)
    {
        Log.Error(ex, "Address already in use.");
        await app.StopAsync();
        ShowErrorDialog("错误！", "端口已被占用，请修改程序目录下appsettings.json文件中Urls的端口号");
    }
    Log.CloseAndFlush();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly.");
    ShowErrorDialog("错误！", "程序启动时出错，请检查日志");
    Log.CloseAndFlush();
    Environment.Exit(1);
}

static async Task OpenUIIfNotSilentStartAsync(WebApplication app, string serverUrl)
{
    using var scope = app.Services.CreateScope();
    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    GetAppSettingsResult appSettings = await mediator.Send(new GetAppSettingsQuery());
    if (!appSettings.IsSilentStartEnabled)
    {
        var appUIManager = scope.ServiceProvider.GetRequiredService<IAppUIManager>();
        await appUIManager.OpenUIAsync();
    }
}

static void ShowErrorDialog(string title, string message)
{
    if (OperatingSystem.IsWindows())
    {
        PInvoke.MessageBox(default, message, title, MESSAGEBOX_STYLE.MB_ICONERROR | MESSAGEBOX_STYLE.MB_OK);
    }
}