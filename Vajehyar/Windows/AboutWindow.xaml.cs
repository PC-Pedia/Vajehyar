using System;
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
        private string _currentVersion;

        public string CurrentVersion
        {
            get => _currentVersion;
            set
            {
                _currentVersion = value;
                NotifyPropertyChanged("CurrentVersion");
            }
        }

        public AboutWindow()
        {
            InitializeComponent();

            Version version = Assembly.GetEntryAssembly()?.GetName().Version;
            CurrentVersion = version.IsBeta() ? "نسخۀ آزمایشی" : GetSemVer(version);

            if (Settings.Default.AboutLeftPos == 0 && Settings.Default.AboutTopPos == 0)
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

        }

        private string GetSemVer(Version ver)
        {
            int major = ver.Major;
            int minor = ver.Minor;
            int patch = ver.Build;

            return $"{major}.{minor}.{patch}";
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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            MinimizeWindow();
            Process.Start(Settings.Default.IDPay);
            e.Handled = true;
        }

        private void MinimizeWindow()
        {
            Close();
            App app = ((App)Application.Current);
            app.HideMainWindow();
        }

        private void EmailIcon_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("mailto:kokabi1365@gmail.com?subject=واژه‌یار");
            e.Handled = true;
        }

        private void GithubIcon_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Settings.Default.GithubUrl);
            e.Handled = true; 
        }

        private void VirgoolIcon_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(Settings.Default.VirgoolUrl);
            e.Handled = true;
        }
    }
}
