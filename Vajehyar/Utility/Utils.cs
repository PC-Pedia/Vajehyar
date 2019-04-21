using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Vajehyar.Utility
{
    public static class Utils
    {
        public static void SendToSystemTray(Window window)
        {
            window.Hide();
            window.WindowState = WindowState.Minimized;
        }

        public static void ExitFromSystemTray(Window window)
        {
            /*window.WindowState = WindowState.Normal;
            window.Show();
            window.txtSearch.SelectAll();
            window.Datagrid.UnselectAllCells();*/
        }
    }
}
