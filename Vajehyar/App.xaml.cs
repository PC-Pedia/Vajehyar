using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.Win32;
using Vajehyar.Properties;
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

        void App_Startup(object sender, StartupEventArgs e)
        {

            kh = new KeyboardHook();

            kh.SetHook();

            kh.OnKeyDownEvent += OnHookKeyDown;

            System.IO.Stream ico = GetResourceStream(new Uri("pack://application:,,,/Resources/Vajehyar.ico")).Stream;
            nIcon.Icon = new Icon(ico);
            nIcon.Visible = true;

            //nIcon.ShowBalloonTip(5000, "Title", "Text", System.Windows.Forms.ToolTipIcon.Info);

            nIcon.MouseDown += NIcon_MouseDown;

            // Application is running
            // Process command line args
            bool startedByWindows = e.Args.Any(s=>s.Contains(Settings.Default.StartupArgument));

            // Create main application window, starting minimized if specified
            MainWindow mainWindow = new MainWindow();
            if (startedByWindows || Settings.Default.StartMinimized)
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

        private bool allKeyPressed(System.Windows.Forms.KeyEventArgs e)
        {
            string[] sArray = Settings.Default.ShortcutKey.Split('+');
            List<Keys> keys=new List<Keys>();
            bool allKeyPressed = false;

            foreach (string key in sArray)
            {
                Enum.TryParse<Keys>(key,out var k);
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
            if (MainWindow.WindowState == WindowState.Normal || Application.Current.Windows.OfType<SettingsWindow>().Any())
            {
                return;
            }


            if (allKeyPressed(e))
            {
                ((ContextMenu)FindResource("NotifierContextMenu")).IsOpen = false;
                ((MainWindow)Current.MainWindow).txtSearch.Focus();
                ((MainWindow)Current.MainWindow).dgvWords.UnselectAllCells();
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Show();
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

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();

            string keyName = Current.MainWindow.GetType().Assembly.GetName().Name; //Application Name: Vajehyar
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
