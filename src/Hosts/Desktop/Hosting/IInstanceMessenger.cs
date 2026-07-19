namespace ScreenTimeTracker.Hosts.Desktop.Hosting;

public interface IInstanceMessenger : IDisposable
{
    event EventHandler<MessageReceivedEventArgs>? MessageReceived;
    Task StartListeningAsync(CancellationToken cancellationToken = default);
    Task StopListeningAsync(CancellationToken cancellationToken = default);
    Task<bool> SendMessageAsync(string message, CancellationToken cancellationToken = default);

}

public class MessageReceivedEventArgs(string message) : EventArgs
{
    public string Message { get; } = message;
}