using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Vajehyar.Properties;

namespace Vajehyar.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            
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

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Close();
            var app = ((App) Application.Current);
            app.HideMainWindow();
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;

        }
    }
}
