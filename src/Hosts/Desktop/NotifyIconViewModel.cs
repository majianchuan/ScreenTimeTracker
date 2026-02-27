using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;
using System.Diagnostics;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

public partial class NotifyIconViewModel(
    IOptions<NotifyIconOptions> options,
    IServiceProvider serviceProvider) : ObservableObject
{
    [RelayCommand]
    public static void OpenAppDirectory()
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = AppContext.BaseDirectory,
            UseShellExecute = true
        });
    }

    [RelayCommand]
    public static void ExitApplication()
    {
        Application.Current.Shutdown();
    }

    [RelayCommand]
    public void OpenUIInBrowser()
    {
        string url = options.Value.UIUrl;

        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }

    [RelayCommand]
    public void OpenUIInWindow()
    {
        if (App.Current.MainWindow is MainView mainView)
        {
            mainView.Show();
            mainView.Activate();
            return;
        }
        string url = options.Value.UIUrl;
        mainView = serviceProvider.GetRequiredService<MainView>();
        mainView.LoadUrl(url);
        mainView.Show();
        App.Current.MainWindow = mainView;
    }

    [RelayCommand]
    public async Task OpenUI()
    {
        using var scope = serviceProvider.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        GetUserSettingsResult userSettings = await mediator.Send(new GetUserSettingsQuery());
        if (userSettings.UIOpenMode == "Browser")
            OpenUIInBrowser();
        else
            OpenUIInWindow();
    }
}
