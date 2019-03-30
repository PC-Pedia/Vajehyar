using System;
using System.Drawing;
using System.Windows;

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
            nIcon.Click += nIcon_Click;           
        }        

        private void OnHookKeyDown(object sender, HookEventArgs e)
        {
            if (MainWindow.WindowState==WindowState.Normal)            
                return;
            
            if (e.Alt && e.Shift && e.Key==System.Windows.Forms.Keys.V)
            {
                MainWindow.Show();
                MainWindow.WindowState = WindowState.Normal;
                ((MainWindow)Current.MainWindow).txtSearch.Focus();
                ((MainWindow)Current.MainWindow).dgvWords.UnselectAllCells();
            }
        }

        private void nIcon_Click(object sender, EventArgs e)
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
}
