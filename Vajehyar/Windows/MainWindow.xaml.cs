﻿using System;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Vajehyar.Properties;
using Vajehyar.Utility;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Vajehyar.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private Database database;

        /*private static FontFamily DefaultFont = new FontFamily(
            new Uri("pack://application:,,,/Resources/Fonts/"),
            "./#IRANSans"
        );*/

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

#if (!DEBUG)
            CheckUpdate();
#endif
        }

        private async void CheckUpdate()
        {
            if (Settings.Default.CheckUpdate)
                await GithubHelper.CheckUpdate();
        }

        private string _filterString;

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
            if (string.IsNullOrEmpty(_filterString))
                return false;

            string str = obj as string;
            string pattern = _filterString;

            if (WholeWord.IsChecked==true)
            {
                pattern = @"\b" + _filterString + @"\b";
            }
            
            return Regex.IsMatch(str, pattern);
        }

        #region Events

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
       

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                txtSearch.SelectAll();
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private void TxtSearch_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
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

        private async void TxtSearch_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (await txtSearch.GetIdle(Settings.Default.SearchDelay))
            {
                FilterString = txtSearch.Text;
            }
        }

        private void RadioButton_OnChecked(object sender, RoutedEventArgs e)
        {
            FilterString = txtSearch.Text;
        }

        private void TxtSearch_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, @"^[\u0600-\u06FF\uFB8A\u067E\u0686\u06AF\u200C\u200F]+$"))
            {
                e.Handled = true;
            }
        }

        private void MainWindow_OnDeactivated(object sender, EventArgs e)
        {
            if (!Settings.Default.MinimizeWhenClickOutside)
                return;

            if (!Settings.Default.ShowInTaskbar)
                Hide();

            WindowState = WindowState.Minimized;
        }
    }

    public class DefaultFontConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return value.ToString().Contains("فونت پیش‌فرض") || string.IsNullOrEmpty(value.ToString()) ? parameter : value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

}
