using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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
            textboxHint.Text = $"جستجو در بین {list.Count} کلمه";

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
            
                /*e.Cancel = true;
                this.Visibility = Visibility.Hidden;*/
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
