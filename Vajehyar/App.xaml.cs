using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Vajehyar.Properties;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Vajehyar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        private KeyboardHook _hook;


        void App_Startup(object sender, StartupEventArgs e)
        {
            _hook = new KeyboardHook();
            _hook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);

            System.IO.Stream ico = GetResourceStream(new Uri("pack://application:,,,/Resources/Vajehyar.ico")).Stream;
            nIcon.Icon = new Icon(ico);
            nIcon.Visible = true;

            //nIcon.ShowBalloonTip(5000, "Title", "Text", System.Windows.Forms.ToolTipIcon.Info);

            nIcon.MouseDown += NIcon_MouseDown;

            // Application is running
            // Process command line args
            bool startMinimized = e.Args.Any(s=>s.Contains(Settings.Default.StartupArgument));

            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow();
            if (startMinimized)
            {
                mainWindow.Hide();
                mainWindow.WindowState = WindowState.Minimized;
                
            }
            else
            {
                mainWindow.Show();
            }
            
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
                    MainWindow.WindowState = WindowState.Normal;
                    MainWindow.Show();
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
                string selectedText = getSelectedText();
                Thread.Sleep(1000);
                ((ContextMenu)FindResource("NotifierContextMenu")).IsOpen = false;
                MainWindow.Show();
                MainWindow.WindowState = WindowState.Normal;
                ((MainWindow)Current.MainWindow).txtSearch.Text = selectedText;
                ((MainWindow)Current.MainWindow).txtSearch.Focus();
                ((MainWindow)Current.MainWindow).dgvWords.UnselectAllCells();               
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(System.Windows.Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Windows.Point lpPoint);

        public static IntPtr GetWindowUnderCursor()
        {
            System.Windows.Point ptCursor = new System.Windows.Point();

            if (!(GetCursorPos(out ptCursor)))
                return IntPtr.Zero;

            return WindowFromPoint(ptCursor);
        }

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        private string getSelectedText()
        {
            SetForegroundWindow(GetWindowUnderCursor()); 
            Thread.Sleep(1000);
            System.Windows.Forms.SendKeys.SendWait("^C"); 

            return Clipboard.GetText(TextDataFormat.UnicodeText);
        }
        

        private void Menu_Settings(object sender, RoutedEventArgs e)
        {
            Window settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void Menu_Help(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_Contact(object sender, RoutedEventArgs e)
        {
            Window contactWindow = new Contact();
            contactWindow.Show();
        }

        private void Menu_About(object sender, RoutedEventArgs e)
        {
            Window aboutWindow = new About();            
            aboutWindow.Show();
        }


        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }
    }
}
