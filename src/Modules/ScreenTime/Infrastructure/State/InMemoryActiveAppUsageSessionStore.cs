using ScreenTimeTracker.Modules.ScreenTime.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.State;

public class InMemoryActiveAppUsageSessionStore : IActiveAppUsageSessionStore
{
    private ActiveAppUsageSessionState? _current;
    public ActiveAppUsageSessionState? Current
    {
        get => Volatile.Read(ref _current);
        set => Interlocked.Exchange(ref _current, value);
    }
}