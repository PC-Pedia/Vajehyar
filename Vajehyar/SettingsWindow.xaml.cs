using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Vajehyar.Properties;
using swf = System.Windows.Forms;

namespace Vajehyar
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
      
        public SettingsWindow()
        {
            InitializeComponent();
            checkbox.IsChecked = isRegKeyExist();

        }
       

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

      
        private void TextBox_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                                     || key == Key.LeftCtrl || key == Key.RightCtrl
                                     || key == Key.LeftAlt || key == Key.RightAlt
                                     || key == Key.LWin || key == Key.RWin
                                     || key == Key.Escape)
            {
                return;
            }

            // Build the shortcut key name.
            StringBuilder shortcutText = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                shortcutText.Append("Shift+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt+");
            }
            shortcutText.Append(key.ToString());

            // Update the text box.
            textBox.Text = shortcutText.ToString();
        }

        private void CheckBox_StartUp_OnChecked(object sender, RoutedEventArgs e)
        {

            string keyName = Application.Current.MainWindow.GetType().Assembly.GetName().Name; //Application Name: Vajehyar
            string value = Assembly.GetExecutingAssembly().Location + " " + Settings.Default.StartupArgument;

            RegistryKey key = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (checkbox.IsChecked == true)
                key.SetValue(keyName, value);
            else
                key.DeleteValue(keyName, false);
        }

        private bool isRegKeyExist()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            return (key.GetValueNames().Contains("Vajehyar"));
        }
    }

   




}
