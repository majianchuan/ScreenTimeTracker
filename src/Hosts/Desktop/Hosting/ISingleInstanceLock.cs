namespace ScreenTimeTracker.Hosts.Desktop.Hosting;

public interface ISingleInstanceLock
{
    bool TryAcquire();
}