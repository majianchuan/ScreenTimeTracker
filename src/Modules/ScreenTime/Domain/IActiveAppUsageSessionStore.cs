namespace ScreenTimeTracker.Modules.ScreenTime.Domain;

public interface IActiveAppUsageSessionStore
{
    ActiveAppUsageSessionState? Current { get; set; }
}