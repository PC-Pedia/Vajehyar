using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using Vajehyar.Properties;
using Vajehyar.Utility;
using Vajehyar.Windows;
using ContextMenu = System.Windows.Controls.ContextMenu;
using Point = System.Windows.Point;

namespace Vajehyar
{
    public partial class App
    {
        private NotifyIcon _notifyIcon;
        private Mutex _mutex;
        private MainWindow _mainWindow;
        private List<Keys> _keys;
        private KeyboardHook _keyboardHook;
        private string _appName;

        private void App_Startup(object sender, StartupEventArgs e)
        {
            //Get application name: Vajehyar
            _appName = Assembly.GetExecutingAssembly().GetName().Name; 

            //Run only one instance of the app
            _mutex = new Mutex(true, _appName, out var createdNew);
            if (!createdNew) Current.Shutdown();

            _notifyIcon = new NotifyIcon();
            _keys = new List<Keys>();

            _keyboardHook = new KeyboardHook();
            _keyboardHook.SetHook();
            _keyboardHook.OnKeyDownEvent += OnHookKeyDown;

            

            

            

            var ico = GetResourceStream(new Uri("pack://application:,,,/Resources/Icons/Vajehyar.ico"))?.Stream;
            _notifyIcon.Icon = new Icon(ico);
            _notifyIcon.Visible = true;

            if (Settings.Default.FirstRun)
            {
                _notifyIcon.ShowBalloonTip(15000, "واژه‌یار", @"با کلیدهای Alt + Shift + V برنامه را باز کنید و با Esc کمینه کنید. این کلیدها را می‌توانید در تنظیمات برنامه تغییر دهید.", ToolTipIcon.Info);
            }
            

            _notifyIcon.MouseDown += NotifyIcon_MouseDown;

            // Application is running
            // Process command line args
            var startedByWindows = e.Args.Any(s => s.Contains(Settings.Default.StartupArgument));

            // Create main application window, starting minimized if specified
            var data = Database.GetData();
            var numberOfWords = Database.GetCount(string.Concat(data));
            var dataWithCount = new Tuple<string[], int>(data, numberOfWords);
            _mainWindow = new MainWindow(dataWithCount);

            if (startedByWindows || Settings.Default.StartMinimized)
                HideMainWindow();
            else
                ShowMainWindow();
        }

        public void HideMainWindow()
        {
            _mainWindow.Hide();
            _mainWindow.WindowState = WindowState.Minimized;
        }

        public void ShowMainWindow()
        {
            ((ContextMenu) FindResource("NotifierContextMenu")).IsOpen = false;
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Show();
            _mainWindow.txtSearch.Focus();
            _mainWindow.txtSearch.SelectAll();
            _mainWindow.Datagrid.UnselectAllCells();
        }

        private void NotifyIcon_MouseDown(object sender, MouseEventArgs e)
        {
            var menu = (ContextMenu) FindResource("NotifierContextMenu");

            if (e.Button == MouseButtons.Right) menu.IsOpen = menu.IsOpen ? false : true;

            if (e.Button == MouseButtons.Left)
            {
                if (_mainWindow.WindowState == WindowState.Normal)
                    HideMainWindow();
                else if (_mainWindow.WindowState == WindowState.Minimized) ShowMainWindow();
            }
        }

        private bool allKeyPressed(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                var menu = (ContextMenu) FindResource("NotifierContextMenu");
                menu.IsOpen = false;
                return false;
            }

            var sArray = Settings.Default.ShortcutKey.Split('+');
            var keys = new List<Keys>();
            var allKeyPressed = false;

            foreach (var key in sArray)
            {
                var kk = key;
                if (kk.Contains("Ctrl"))
                    kk = "Control";
                var k = (Keys) Enum.Parse(typeof(Keys), kk);
                keys.Add(k);
            }

            switch (keys.Count)
            {
                case 1:
                    if (e.KeyData == keys[0])
                        allKeyPressed = true;
                    break;
                case 2:
                    if (e.KeyData == (keys[0] | keys[1])) allKeyPressed = true;

                    break;

                case 3:
                    if (e.KeyData == (keys[0] | keys[1] | keys[2]))
                        allKeyPressed = true;
                    break;

                case 4:
                    if (e.KeyData == (keys[0] | keys[1] | keys[2] | keys[3]))
                        allKeyPressed = true;
                    break;
            }

            return allKeyPressed;
        }

        private void OnHookKeyDown(object sender, KeyEventArgs e)
        {
            if (_mainWindow.WindowState == WindowState.Normal || Current.Windows.OfType<SettingWindow>().Any()) return;

            if (allKeyPressed(e)) ShowMainWindow();
        }

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point lpPoint);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        public static IntPtr GetWindowUnderCursor()
        {
            var ptCursor = new Point();

            if (!GetCursorPos(out ptCursor))
                return IntPtr.Zero;

            return WindowFromPoint(ptCursor);
        }

        private void Menu_Settings(object sender, RoutedEventArgs e)
        {
            Window settingsWindow = new SettingWindow();
            settingsWindow.Show();
        }

        private void Menu_Help(object sender, RoutedEventArgs e)
        {
        }

        private void Menu_Contact(object sender, RoutedEventArgs e)
        {
        }

        private void Menu_About(object sender, RoutedEventArgs e)
        {
            Window aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }


        private void Menu_Exit(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();

            var keyName = _appName; //Application Name: Vajehyar
            var value = Assembly.GetExecutingAssembly().Location + " " + Settings.Default.StartupArgument;

            var key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (Settings.Default.StartByWindows)
                key.SetValue(keyName, value);
            else
                key.DeleteValue(keyName, false);

            _notifyIcon.Dispose();
        }
    }
}