namespace ScreenTimeTracker.Hosts.Desktop.Hosting;

public interface ISingleInstanceLock : IDisposable
{
    bool TryAcquire();
}