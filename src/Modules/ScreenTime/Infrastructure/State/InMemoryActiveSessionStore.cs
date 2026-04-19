using ScreenTimeTracker.Modules.ScreenTime.Domain;

namespace ScreenTimeTracker.Modules.ScreenTime.Infrastructure.State;

public class InMemoryActiveSessionStore : IActiveSessionStore
{
    private ActiveSessionState? _current;
    public ActiveSessionState? Current
    {
        get => Volatile.Read(ref _current);
        set => Interlocked.Exchange(ref _current, value);
    }
}