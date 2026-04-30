using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace ScreenTimeTracker.Hosts.Desktop;

public interface IViewFactory
{
    T Create<T>() where T : Window;
}

public class ViewFactory(IServiceProvider provider) : IViewFactory
{
    private readonly IServiceProvider _provider = provider;

    public T Create<T>() where T : Window
    {
        var scope = _provider.CreateScope();
        var view = ActivatorUtilities.CreateInstance<T>(scope.ServiceProvider);
        view.Closed += (sender, args) =>
        {
            scope.Dispose();
        };
        return view;
    }
}