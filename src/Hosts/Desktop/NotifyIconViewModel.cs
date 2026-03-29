using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using ScreenTimeTracker.Modules.Shell.Features.UserSettingsManagement.GetUserSettings;
using System.Diagnostics;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

public partial class NotifyIconViewModel(
    IServiceScopeFactory scopeFactory,
    Func<MainView> mainViewFactory) : ObservableObject
{
    public string UIUrl { get; set; } = string.Empty;

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
        Process.Start(new ProcessStartInfo
        {
            FileName = UIUrl,
            UseShellExecute = true
        });
    }

    [RelayCommand]
    public void OpenUIInWindow()
    {
        if (Application.Current.MainWindow is not MainView mainView)
        {
            mainView = mainViewFactory();
            mainView.LoadUrl(UIUrl);
            Application.Current.MainWindow = mainView;
        }
        else if (mainView.WindowState == WindowState.Minimized)
        {
            mainView.WindowState = WindowState.Normal;
        }
        mainView.Show();
        mainView.Activate();
    }

    [RelayCommand]
    public async Task OpenUI()
    {
        using var scope = scopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        GetUserSettingsResult userSettings = await mediator.Send(new GetUserSettingsQuery());
        if (userSettings.UIOpenMode == UIOpenModeDto.Browser)
            OpenUIInBrowser();
        else
            OpenUIInWindow();
    }
}
