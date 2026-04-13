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
    private bool _isShutdownInitiatedByHost = false;

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

                _wpfApp.Exit += (s, e) =>
                {
                    logger.LogInformation("WPF Application Exited.");
                    if (!_isShutdownInitiatedByHost)
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

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StoppingAsync(CancellationToken cancellationToken)
    {
        _isShutdownInitiatedByHost = true;
        if (_wpfApp is not null && !_wpfApp.Dispatcher.HasShutdownStarted)
            _wpfApp.Dispatcher.Invoke(_wpfApp.Shutdown);
        await Task.Run(() => _wpfThread?.Join(TimeSpan.FromSeconds(3)), cancellationToken);
        return;
    }
}
