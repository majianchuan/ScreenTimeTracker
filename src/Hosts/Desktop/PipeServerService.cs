using System.IO.Pipes;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScreenTimeTracker.Hosts.Desktop;

public class PipeServerService(
    IAppUIManager appUIManager,
    ILogger<PipeServerService> logger
) : BackgroundService
{
    public static readonly string PipeName = "ScreenTimeTrackerDesktopPipe";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var server = new NamedPipeServerStream(PipeName);

            await server.WaitForConnectionAsync(stoppingToken);

            byte[] buffer = new byte[256];
            int read = await server.ReadAsync(buffer, stoppingToken);

            string msg = Encoding.UTF8.GetString(buffer, 0, read);

            if (msg == "SHOW")
            {
                logger.LogInformation("Received SHOW command.");
                await appUIManager.OpenUIAsync();
            }
        }
    }
}
