using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Vajehyar.Utility;

namespace Vajehyar.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string _filterString;
        private string _str;
        private ICollectionView _lines;
        public ICollectionView Lines
        {
            get => _lines;
            set { _lines = value; NotifyPropertyChanged("Lines"); }
        }

        public MainWindow(string[] lines)
        {
            InitializeComponent();
            Lines = CollectionViewSource.GetDefaultView(lines);
            Lines.Filter = FilterResult;
            textboxHint.Text = $"جستجوی فارسی بین {Utils.RoundNumber(Lines.Count())} واژه";
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

        public bool FilterResult(Object obj)
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
            App.nIcon.Dispose();
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
            if (!Regex.IsMatch(e.Text, @"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\u200C\u200F ]+$"))
            {
                BlinkText(textboxHint);
                e.Handled = true;
            }


        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                //Grid_MouseDown(null, null);
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
                case "ContactButton":
                    new ContactWindow().Show();
                    break;
            }

        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
