namespace ScreenTimeTracker.Hosts.Desktop;

public interface IViewFactory
{
    T Create<T>() where T : class;
}
