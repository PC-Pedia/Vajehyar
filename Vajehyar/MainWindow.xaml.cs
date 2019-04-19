using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Vajehyar.ViewModel;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Vajehyar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public LineViewModel LineViewModel { get; }

        public MainWindow(LineViewModel vm)
        {
            InitializeComponent();
            LineViewModel = vm;
            DataContext = LineViewModel;
           
            LineViewModel.Lines.Filter = LineViewModel.FilterResult;
            textboxHint.Text = $"جستجوی فارسی بین {RoundNumber(LineViewModel.Lines.Count())} واژه";           
        }

        private int RoundNumber(int num)
        {
            return num % 1000 >= 500 ? num + 1000 - num % 1000 : num - num % 1000;
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LineViewModel.FilterString = txtSearch.Text;

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

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                //Grid_MouseDown(null, null);
            }
        }
        
    }

}
