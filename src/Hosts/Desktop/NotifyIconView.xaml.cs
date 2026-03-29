using System.Windows;
using System.Windows.Controls;

namespace ScreenTimeTracker.Hosts.Desktop
{
    /// <summary>
    /// NotifyIconView.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyIconView : UserControl, IDisposable
    {
        public NotifyIconView(NotifyIconViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            TrayIcon.ForceCreate();
        }

        public void Dispose()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TrayIcon?.Dispose();
            });
            GC.SuppressFinalize(this);
        }
    }
}
