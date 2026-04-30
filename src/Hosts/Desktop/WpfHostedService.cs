using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScreenTimeTracker.Hosts.Desktop;

public class WpfHostedService(
    ILogger<WpfHostedService> logger,
    IHostApplicationLifetime lifetime
) : IHostedLifecycleService
{
    private Thread? _wpfThread;
    private App? _wpfApp;
    private volatile bool _isExitInitiatedByHost = false;
    private volatile bool _isExitInitiatedByEndSession = false;
    private readonly TaskCompletionSource _hostStopped = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StartingAsync(CancellationToken cancellationToken)
    {
        var wpfReady = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _wpfThread = new Thread(() =>
        {
            try
            {
                _wpfApp = new App();
                _wpfApp.InitializeComponent();

                _wpfApp.SessionEnding += (_, _) =>
                {
                    _isExitInitiatedByEndSession = true;
                    logger.LogInformation("WPF Session Ending. Stopping WebHost...");
                    lifetime.StopApplication();
                    // 关机时阻塞等待 WebHost 停止
                    _hostStopped.Task.Wait(TimeSpan.FromSeconds(3));
                };

                _wpfApp.Exit += (_, _) =>
                {
                    logger.LogInformation("WPF Application Exited.");
                    if (!_isExitInitiatedByHost && !_isExitInitiatedByEndSession)
                    {
                        logger.LogInformation("Shutting down WebHost...");
                        lifetime.StopApplication();
                    }
                };

                _wpfApp.Startup += (_, _) => wpfReady.TrySetResult();
                _wpfApp.Run();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WPF UI Thread crashed.");
                wpfReady.TrySetException(ex);
                lifetime.StopApplication();
            }
        });
        _wpfThread.SetApartmentState(ApartmentState.STA);
        _wpfThread.Name = "WPF UI Thread";
        _wpfThread.Start();

        return wpfReady.Task;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _isExitInitiatedByHost = true;
        if (!_isExitInitiatedByEndSession && _wpfApp is not null && !_wpfApp.Dispatcher.HasShutdownStarted)
            await _wpfApp.Dispatcher.InvokeAsync(_wpfApp.Shutdown);
    }

    public async Task StoppedAsync(CancellationToken cancellationToken) => _hostStopped.TrySetResult();
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
