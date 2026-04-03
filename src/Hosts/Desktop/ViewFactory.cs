using Microsoft.Extensions.DependencyInjection;

namespace ScreenTimeTracker.Hosts.Desktop;

public class ViewFactory(IServiceProvider provider) : IViewFactory
{
    private readonly IServiceProvider _provider = provider;

    T IViewFactory.Create<T>()
    {
        return ActivatorUtilities.CreateInstance<T>(_provider);
    }
}