using Microsoft.Extensions.Logging;
using ScreenTimeTracker.Hosts.Desktop.Hosting;
using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;

namespace ScreenTimeTracker.Hosts.Desktop.Platforms;

[SupportedOSPlatform("windows")]
public class WindowsInstanceMessenger(ILogger<WindowsInstanceMessenger> logger) : IInstanceMessenger
{
    private static readonly string _pipeName = "ScreenTimeTrackerPipe";
    private bool _disposed = false;
    private CancellationTokenSource? _cts;
    private Task? _listeningTask;

    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;


    public async Task<bool> SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        try
        {
            using var clientStream = new NamedPipeClientStream(".", _pipeName);
            await clientStream.ConnectAsync(cancellationToken);
            byte[] msg = Encoding.UTF8.GetBytes(message);
            await clientStream.WriteAsync(msg.AsMemory(), cancellationToken);
            await clientStream.FlushAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send message to instance.");
            return false;
        }
    }

    public Task StartListeningAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_listeningTask is not null && !_listeningTask.IsCompleted)
            return Task.CompletedTask;

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _listeningTask = Task.Run(() => ListenLoopAsync(_cts.Token), _cts.Token);

        return Task.CompletedTask;
    }

    private async Task ListenLoopAsync(CancellationToken cancellationToken)
    {
        PipeSecurity pipeSecurity = new();
        // 允许所有用户读写
        pipeSecurity.AddAccessRule(new PipeAccessRule(
            new SecurityIdentifier(WellKnownSidType.WorldSid, null),
            PipeAccessRights.ReadWrite,
            AccessControlType.Allow
        ));
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                using var serverStream = NamedPipeServerStreamAcl.Create(
                     pipeName: _pipeName,
                     direction: PipeDirection.In,
                     maxNumberOfServerInstances: 1,
                     transmissionMode: PipeTransmissionMode.Byte,
                     options: PipeOptions.Asynchronous,
                     inBufferSize: 0,
                     outBufferSize: 0,
                     pipeSecurity: pipeSecurity
                 );
                await serverStream.WaitForConnectionAsync(cancellationToken);
                using var reader = new StreamReader(serverStream, Encoding.UTF8, leaveOpen: true);
                string msg = await reader.ReadToEndAsync(cancellationToken);
                // 异步触发事件,避免外部订阅者的同步阻塞拖慢监听循环
                var handler = MessageReceived;
                if (handler != null)
                {
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            handler(this, new MessageReceivedEventArgs(msg));
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "An unhandled exception occurred in the MessageReceived event handler.");
                        }
                    }, CancellationToken.None);
                }
                serverStream.Disconnect();
            }
            catch (OperationCanceledException)
            {
                // 正常取消监听，退出循环
                break;
            }
            catch (Exception ex)
            {
                // 发生异常时等待一下，防止死循环导致 CPU 占用过高
                logger.LogError(ex, "An error occurred while listening for messages.");
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public async Task StopListeningAsync(CancellationToken cancellationToken = default)
    {
        _cts?.Cancel();
        if (_listeningTask is not null)
        {
            try
            {
                await _listeningTask;
            }
            catch (OperationCanceledException)
            {
                // 忽略取消异常
            }
            _listeningTask = null;
        }
        _cts?.Dispose();
        _cts = null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        _disposed = true;
    }
}