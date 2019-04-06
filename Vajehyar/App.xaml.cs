using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Vajehyar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        private KeyboardHook _hook;
        

        public App()
        {
            _hook = new KeyboardHook();
            _hook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);

            System.IO.Stream ico = GetResourceStream(new Uri("pack://application:,,,/Resources/Vajehyar.ico")).Stream;
            nIcon.Icon = new Icon(ico);
            nIcon.Visible = true;
           
            //nIcon.ShowBalloonTip(5000, "Title", "Text", System.Windows.Forms.ToolTipIcon.Info);

            nIcon.MouseDown += NIcon_MouseDown;

        }

        private void NIcon_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ContextMenu menu = (ContextMenu)FindResource("NotifierContextMenu");

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                menu.IsOpen = menu.IsOpen ? false : true;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (MainWindow.WindowState == WindowState.Normal)
                {
                    MainWindow.Hide();
                    MainWindow.WindowState = WindowState.Minimized;
                }
                else if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.Show();
                    MainWindow.WindowState = WindowState.Normal;
                    ((MainWindow)Current.MainWindow).txtSearch.Focus();
                    ((MainWindow)Current.MainWindow).dgvWords.UnselectAllCells();
                }
            }
        }

        private void OnHookKeyDown(object sender, HookEventArgs e)
        {
            if (MainWindow.WindowState == WindowState.Normal)
            {
                return;
            }
           

            if (e.Alt && e.Shift && e.Key == System.Windows.Forms.Keys.V)
            {
                ((ContextMenu)FindResource("NotifierContextMenu")).IsOpen = false;
                MainWindow.Show();
                MainWindow.WindowState = WindowState.Normal;
                ((MainWindow)Current.MainWindow).txtSearch.Focus();
                ((MainWindow)Current.MainWindow).dgvWords.UnselectAllCells();
            }
        }        

        private void Menu_Settings(object sender, RoutedEventArgs e)
        {
            
        }

        private void Menu_Help(object sender, RoutedEventArgs e)
        {
            
        }

        private void Menu_Contact(object sender, RoutedEventArgs e)
        {
            
        }

        private void Menu_About(object sender, RoutedEventArgs e)
        {
            /*Window aboutWindow = new About();            
            aboutWindow.Show();*/
        }

        private void Menu_Update(object sender, RoutedEventArgs e)
        {
            
        }

        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();            
        }
    }
}
