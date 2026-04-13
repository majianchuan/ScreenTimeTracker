using Serilog;
using System.Windows;

namespace ScreenTimeTracker.Hosts.Desktop;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        DispatcherUnhandledException += (_, e) =>
        {
            e.Handled = true; // 阻止 WPF 硬崩溃
            Log.Fatal(e.Exception, "WPF UI thread terminated unexpectedly.");
            MessageBox.Show("程序发生未知错误，即将退出", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            // 引导程序走向正常的 OnExit 退出流程
            if (!Current.Dispatcher.HasShutdownStarted)
                Current.Shutdown();
        };
        SessionEnding += (_, _) =>
        {
            Log.Information("System shutting down.");
        };
    }
}

