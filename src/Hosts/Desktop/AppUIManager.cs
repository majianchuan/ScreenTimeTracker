using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Mediator;
using ScreenTimeTracker.Modules.AppBehavior.Features.UserPreferencesManagement.GetUserPreferences;

namespace ScreenTimeTracker.Hosts.Desktop;

public interface IAppUIManager
{
    Task OpenUIAsync();
    Task OpenUIInWindowAsync();
    Task OpenUIInBrowserAsync();
}

public class AppUIManager(
    IServiceScopeFactory scopeFactory,
    IServerUrlProvider urlProvider,
    IViewFactory viewFactory) : IAppUIManager
{
    public async Task OpenUIAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var settings = await mediator.Send(new GetUserPreferencesQuery());

        if (settings.DefaultUIOpenMode == UIOpenModeDto.Browser)
            await OpenUIInBrowserAsync();
        else
            await OpenUIInWindowAsync();
    }

    public async Task OpenUIInBrowserAsync()
    {
        string uiUrl = urlProvider.GetServerUrl();
        Process.Start(new ProcessStartInfo
        {
            FileName = uiUrl,
            UseShellExecute = true
        });
    }

    public async Task OpenUIInWindowAsync()
    {
        string uiUrl = urlProvider.GetServerUrl();
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (Application.Current.MainWindow is not MainView mainView)
            {
                mainView = viewFactory.Create<MainView>();
                mainView.LoadUrl(uiUrl);
                Application.Current.MainWindow = mainView;
            }
            else if (mainView.WindowState == WindowState.Minimized)
            {
                mainView.WindowState = WindowState.Normal;
            }

            mainView.Show();
            mainView.Activate();

            // 解决 Windows 偶尔不置顶的问题
            if (!mainView.Topmost)
            {
                mainView.Topmost = true;
                mainView.Topmost = false;
            }
        });
    }
}