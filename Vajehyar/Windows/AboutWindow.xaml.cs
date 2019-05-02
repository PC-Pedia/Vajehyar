using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Vajehyar.Properties;
using Vajehyar.Utility;

namespace Vajehyar.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow
    /// </summary>
    public partial class AboutWindow : INotifyPropertyChanged
    {
        private string _releaseVersion;

        public string ReleaseVersion
        {
            get => _releaseVersion;
            set
            {
                _releaseVersion = value;
                NotifyPropertyChanged("ReleaseVersion");
            }
        }

        public AboutWindow()
        {
            InitializeComponent();
            ReleaseVersion = Assembly.GetEntryAssembly()?.GetName().Version.ToString();

            if (Settings.Default.AboutLeftPos==0 && Settings.Default.AboutTopPos==0)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Close();
            var app = ((App) Application.Current);
            app.HideMainWindow();
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;

        }

        private void Version_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(Settings.Default.UpdateUrl);
            e.Handled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
