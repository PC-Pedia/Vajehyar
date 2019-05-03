﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;
using Vajehyar.Properties;
using Vajehyar.Utility;
using Vajehyar.Windows;
using ContextMenu = System.Windows.Controls.ContextMenu;

namespace Vajehyar
{
    public partial class App
    {
        #region Fields
        private NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;
        private KeyboardHook _keyboardHook;
        private string _appName;
        private ContextMenu _contextMenu;
        #endregion

        #region App Startup
        private void App_Startup(object sender, StartupEventArgs e)
        {
            //Get application name: Vajehyar
            _appName = Assembly.GetExecutingAssembly().GetName().Name;
            _contextMenu = FindResource("NotifierContextMenu") as ContextMenu;

            //Run only one instance of the app
            var mutex = new Mutex(true, _appName, out var createdNew);
            if (!createdNew) Current.Shutdown();

            SetKeyboardHook();
            SetNotifyIcon();

            _mainWindow = new MainWindow(Database.Instance);
            var hasStartByWindowsArg = e.Args.Any(s => s.Contains(Settings.Default.StartupArgument));

            //Start minimized if specified
            if (hasStartByWindowsArg || Settings.Default.StartMinimized)
                HideMainWindow();
            else
                ShowMainWindow();
        }

        #endregion

        #region System tray
        private void SetNotifyIcon()
        {
            _notifyIcon = new NotifyIcon();
            var ico = GetResourceStream(new Uri("pack://application:,,,/Resources/Icons/Vajehyar.ico"))?.Stream;
            _notifyIcon.Icon = new Icon(ico);
            _notifyIcon.Visible = true;

            if (Settings.Default.FirstRun)
            {
                _notifyIcon.ShowBalloonTip(30000, "واژه‌یار", "باز کردن برنامه: Alt + Shift + V\nبستن برنامه: Esc\nتنظیمات را می‌توانید تغییر دهید.", ToolTipIcon.Info);
                Settings.Default.FirstRun = false;
            }

            _notifyIcon.MouseDown += NotifyIcon_MouseDown;
        }

        private void NotifyIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) _contextMenu.IsOpen = !_contextMenu.IsOpen;

            if (e.Button == MouseButtons.Left)
            {
                if (_mainWindow.WindowState == WindowState.Normal)
                    HideMainWindow();
                else if (_mainWindow.WindowState == WindowState.Minimized)
                    ShowMainWindow();
            }
        }

        private void Menu_Setting(object sender, RoutedEventArgs e)
        {
            Window settingsWindow = new SettingWindow();
            settingsWindow.Show();
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
        #endregion

        #region Keyboard hook and shortcut key
        private void SetKeyboardHook()
        {
            _keyboardHook = new KeyboardHook();
            _keyboardHook.SetHook();
            _keyboardHook.OnKeyDownEvent += OnHookKeyDown;
        }

        private void OnHookKeyDown(object sender, KeyEventArgs e)
        {
            if (_mainWindow.WindowState == WindowState.Normal || Current.Windows.OfType<SettingWindow>().Any()) return;

            if (IsShortcutKeysPressed(e)) ShowMainWindow();
        }

        private bool IsShortcutKeysPressed(KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                _contextMenu.IsOpen = false;
                return false;
            }

            var splittedKeys = Settings.Default.ShortcutKey.Split('+');
            var keys = new List<Keys>();
            var allKeyPressed = false;

            foreach (var key in splittedKeys)
            {
                var kk = key;
                if (kk.Contains("Ctrl"))
                    kk = "Control";
                var k = (Keys)Enum.Parse(typeof(Keys), kk);
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
        #endregion

        #region Show and hide windows
        public void HideMainWindow()
        {
            _mainWindow.Hide();
            _mainWindow.WindowState = WindowState.Minimized;
        }

        public void ShowMainWindow()
        {
            _contextMenu.IsOpen = false;
            _mainWindow.WindowState = WindowState.Normal;
            _mainWindow.Datagrid.UnselectAllCells();
            _mainWindow.Show();
            _mainWindow.Focus();
            _mainWindow.txtSearch.SelectAll();
            _mainWindow.txtSearch.Focus();

        }
        #endregion

        #region Exit tasks
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
            _notifyIcon.Dispose();
            ConfigRegistry();
        }

        private void ConfigRegistry()
        {
            var key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (!Settings.Default.StartByWindows)
            {
                key?.DeleteValue(_appName, false);
                return;
            }

            var value = Assembly.GetExecutingAssembly().Location + " " + Settings.Default.StartupArgument;
            key?.SetValue(_appName, value);
        } 
        #endregion
    }
}