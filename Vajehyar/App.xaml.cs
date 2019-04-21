using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;
using Vajehyar.Properties;
using Vajehyar.Utility;
using Vajehyar.Windows;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MessageBox = System.Windows.Forms.MessageBox;
using TextDataFormat = System.Windows.TextDataFormat;

namespace Vajehyar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static System.Windows.Forms.NotifyIcon nIcon = new System.Windows.Forms.NotifyIcon();
        KeyboardHook kh;
        List<System.Windows.Forms.Keys> keys = new List<Keys>();
        private static Mutex _mutex = null;
        private MainWindow _mainWindow;

        void App_Startup(object sender, StartupEventArgs e)
        {
            const string appName = "Vajehyar";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                //app is already running! Exiting the application  
                Application.Current.Shutdown();
            }

            kh = new KeyboardHook();
            kh.SetHook();
            kh.OnKeyDownEvent += OnHookKeyDown;

            System.IO.Stream ico = GetResourceStream(new Uri("pack://application:,,,/Resources/Icons/Vajehyar.ico")).Stream;
            nIcon.Icon = new Icon(ico);
            nIcon.Visible = true;

            //nIcon.ShowBalloonTip(5000, "Title", "Text", System.Windows.Forms.ToolTipIcon.Info);

            nIcon.MouseDown += NIcon_MouseDown;

            // Application is running
            // Process command line args
            bool startedByWindows = e.Args.Any(s=>s.Contains(Settings.Default.StartupArgument));

            // Create main application window, starting minimized if specified
            string[] data = Database.GetData();
            int numberOfWords = Database.GetCount(string.Concat(data));
            var dataWithCount=new Tuple<string[], int> (data, numberOfWords);
            _mainWindow = new MainWindow(dataWithCount);
            
            if (startedByWindows || Settings.Default.StartMinimized)
            {
                HideMainWindow();
            }
            else
            {
                ShowMainWindow();
            }
        }

        public void HideMainWindow()
        {
            _mainWindow.Hide();
            _mainWindow.WindowState = WindowState.Minimized;
        }

        public void ShowMainWindow()
        {
            ((ContextMenu)FindResource("NotifierContextMenu")).IsOpen = false;
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Show();
            _mainWindow.txtSearch.Focus();
            _mainWindow.txtSearch.SelectAll();
            _mainWindow.Datagrid.UnselectAllCells();
            
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
                if (_mainWindow.WindowState == WindowState.Normal)
                {
                    HideMainWindow();
                }
                else if (_mainWindow.WindowState == WindowState.Minimized)
                {
                    ShowMainWindow();
                }
            }
        }

        private bool allKeyPressed(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData==Keys.Escape)
            {
                ContextMenu menu = (ContextMenu)FindResource("NotifierContextMenu");
                menu.IsOpen = false;
                return false;
            }

            string[] sArray = Settings.Default.ShortcutKey.Split('+');
            List<Keys> keys=new List<Keys>();
            bool allKeyPressed = false;

            foreach (string key in sArray)
            {
                string kk=key;
                if (kk.Contains("Ctrl"))
                    kk = "Control";
                Keys k = (Keys) Enum.Parse(typeof(Keys), kk);
                keys.Add(k);
            }

            switch (keys.Count)
            {
                case 1:
                    if (e.KeyData == keys[0])
                        allKeyPressed = true;
                 break;
                case 2:
                    if (e.KeyData == (keys[0] | keys[1]))
                    {
                        allKeyPressed = true;
                    }
                        
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

        private void OnHookKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (_mainWindow.WindowState == WindowState.Normal || Application.Current.Windows.OfType<SettingWindow>().Any())
            {
                return;
            }

            if (allKeyPressed(e))
            {
                ShowMainWindow();
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

            string keyName = _mainWindow.GetType().Assembly.GetName().Name; //Application Name: Vajehyar
            string value = Assembly.GetExecutingAssembly().Location + " " + Settings.Default.StartupArgument;

            RegistryKey key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (Settings.Default.StartByWindows)
                key.SetValue(keyName, value);
            else
                key.DeleteValue(keyName, false);
        }
    }


}
