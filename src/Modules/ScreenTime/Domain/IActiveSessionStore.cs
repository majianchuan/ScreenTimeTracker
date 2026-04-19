namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public interface IActiveSessionStore
{
    ActiveSessionState? Current { get; set; }
}