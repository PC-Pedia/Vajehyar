using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Vajehyar.Model;
using Vajehyar.DataLayer;
using Vajehyar.View;

namespace Vajehyar.ViewModel
{
    public class LineViewModel: INotifyPropertyChanged
    {
        private string _filterString;
        private ICollectionView _lines;
        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(param => MyAction(param), _canExecute));
               
            }
        }

        private bool _canExecute;
        public void MyAction(object param)
        {
            string str = param as string;
            switch (str)
            {
                case "Setting":
                    new SettingWindow().ShowDialog();
                    break;
                case "AboutWindow":
                    new AboutWindow().Show();
                    break;
                case "ContactWindow":
                    new ContactWindow().Show();
                    break;
            }
        }

        public LineViewModel()
        {
            Lines = CollectionViewSource.GetDefaultView(Repository.Getlines());
            _canExecute = true;
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

        private String str;

        public bool FilterResult(Object obj)
        {
            str = obj as string;

            if (!string.IsNullOrEmpty(_filterString))
            {
                return Regex.IsMatch(str, @"\b" + _filterString + @"\b");
            }
            return true;
        }


        public ICollectionView Lines
        {
            get => _lines;
            set { _lines = value; NotifyPropertyChanged("Lines"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
