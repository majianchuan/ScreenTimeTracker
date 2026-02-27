using System.Windows.Controls;

namespace ScreenTimeTracker.Hosts.Desktop
{
    /// <summary>
    /// NotifyIconView.xaml 的交互逻辑
    /// </summary>
    public partial class NotifyIconView : UserControl, IDisposable
    {
        private bool _disposed = false;

        public NotifyIconView(NotifyIconViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // 释放托管资源
                TrayIcon?.Dispose();

                // 如果 ViewModel 也实现了 IDisposable
                (DataContext as IDisposable)?.Dispose();
            }

            _disposed = true;
        }

        ~NotifyIconView()
        {
            Dispose(false);
        }
    }
}
