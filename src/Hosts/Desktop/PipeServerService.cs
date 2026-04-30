using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
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
            var pipeSecurity = new PipeSecurity();

            // 允许所有用户读写
            pipeSecurity.AddAccessRule(new PipeAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                PipeAccessRights.ReadWrite,
                AccessControlType.Allow
            ));

            using var server = NamedPipeServerStreamAcl.Create(
                pipeName: PipeName,
                direction: PipeDirection.InOut,
                maxNumberOfServerInstances: 1,
                transmissionMode: PipeTransmissionMode.Byte,
                options: PipeOptions.Asynchronous,
                inBufferSize: 0,
                outBufferSize: 0,
                pipeSecurity: pipeSecurity
            );
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
