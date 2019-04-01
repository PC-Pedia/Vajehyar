using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Vajehyar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ICollectionView _words;
        private string _filterString;

        public MainWindow()
        {
            InitializeComponent();           

            DataContext = this;
            var list = data();
            Words = CollectionViewSource.GetDefaultView(list);
            Words.Filter = new Predicate<object>(Filter);
            //textboxHint.Text = $"جستجو بین {list.Count} کلمه. لطفاً فارسی بنویسید.";            

        }

        public class Word
        {
            public string Definition { get; set; }
            public string Mean { get; set; }
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
            if (_words != null)
            {
                _words.Refresh();
            }
        }

        public bool Filter(object obj)
        {
            Word data = obj as Word;
            if (data != null)
            {
                if (!string.IsNullOrEmpty(_filterString))
                {
                    return data.Definition.Trim().Contains(_filterString);
                }
                return true;
            }
            return false;
        }


        public ICollectionView Words
        {
            get => _words;
            set { _words = value; NotifyPropertyChanged("Words"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public List<Word> data()
        {

            string content = Properties.Resources.words;
            string[] lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            List<Word> words = new List<Word>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(':');
                Word word = new Word
                {
                    Definition = cols[0],
                    Mean = cols[1]
                };
                words.Add(word);


            }

            return words;
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

        private void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Grid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
                       
        }

        private void Window_Activated(object sender, EventArgs e)
        {
           
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState==WindowState.Normal)
            {
                txtSearch.SelectAll();
            }
        }

        private void TxtSearch_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {            
            if (!Regex.IsMatch(e.Text, @"\p{IsArabic}")
        && !string.IsNullOrWhiteSpace(e.Text))
            {
                BlinkText(textboxHint);
                e.Handled = true;
            }
                
            
        }

        private void BlinkText(TextBlock textBlock)
        {
            DoubleAnimation da = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0,0,200)));
            
            da.RepeatBehavior = new RepeatBehavior(2);
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
                Grid_MouseDown(null, null);
            }
        }
    }
}
