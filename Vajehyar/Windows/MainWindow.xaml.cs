using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Vajehyar.Properties;
using Vajehyar.Utility;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Vajehyar.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string _filterString;
        private string _str;

        private bool _hasNewRelease;

        public bool HasNewRelease
        {
            get => _hasNewRelease;
            set
            {
                _hasNewRelease = value;
                NotifyPropertyChanged("HasNewRelease");
            }
        }

        private ICollectionView _lines;
        public ICollectionView Lines
        {
            get => _lines;
            set { _lines = value; NotifyPropertyChanged("Lines"); }
        }

        private string _hint;

        public string Hint
        {
            get => _hint;
            set { _hint = value; NotifyPropertyChanged("Hint"); }
        }

        public MainWindow(Database database)
        {
            InitializeComponent();

            Lines = CollectionViewSource.GetDefaultView(database.Lines);
            Lines.Filter = FilterResult;
            Hint = $"جستجوی فارسی بین {database.GetCount().Round().Format()} واژه";

            Task.Run(() =>
            {
                HasNewRelease = GithuHelper.HasNewRelease;
            });
        }

        public string FilterString
        {
            get => _filterString;
            set
            {
                _filterString = value;
                NotifyPropertyChanged("FilterString");
                FilterCollection();
            }
        }

        private void FilterCollection()
        {
            _lines?.Refresh();
        }

        public bool FilterResult(object obj)
        {
            _str = obj as string;

            if (!string.IsNullOrEmpty(_filterString))
            {
                return Regex.IsMatch(_str, @"\b" + _filterString + @"\b");
            }
            return true;
        }

        private void BlinkText(TextBlock textBlock)
        {
            DoubleAnimation da = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 200)))
            {
                RepeatBehavior = new RepeatBehavior(2)
            };
            //da.AutoReverse = true;
            Storyboard sb = new Storyboard();
            sb.Children.Add(da);
            Storyboard.SetTargetProperty(da, new PropertyPath("(TextBlock.Opacity)"));
            Storyboard.SetTarget(da, textBlock);
            sb.Begin();

        }

        #region Events

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterString = txtSearch.Text;

        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Application.Current.Shutdown();
            Hide();
            WindowState = WindowState.Minimized;
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                txtSearch.SelectAll();
            }
        }

        private void TxtSearch_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\u200C\u200F]+$"))
            {
                BlinkText(textboxHint);
                e.Handled = true;
            }

        }


        private void TopLeftButton_OnClick(object sender, RoutedEventArgs e)
        {
            string name = ((DependencyObject)sender).GetValue(NameProperty) as string;

            switch (name)
            {
                case "SettingButton":
                    new SettingWindow().ShowDialog();
                    break;
                case "AboutButton":
                    new AboutWindow().Show();
                    break;
            }

        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void UpdateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Settings.Default.UpdateUrl);
            e.Handled = true;
        }

        private void TxtSearch_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (InputLanguage.CurrentInputLanguage.LayoutName != null && !InputLanguage.CurrentInputLanguage.LayoutName.ToLower().Contains("persian"))
            {
                return;
            }

            bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            bool ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
            bool space = Keyboard.IsKeyDown(Key.Space);
            bool two = Keyboard.IsKeyDown(Key.D2);

            if (shift && space || ctrl && shift && two)
            {
                e.Handled = true;
                txtSearch.Text += "\u200c";
                txtSearch.SelectionStart = txtSearch.Text.Length;
                txtSearch.SelectionLength = 0;
            }
        }
    }
}
