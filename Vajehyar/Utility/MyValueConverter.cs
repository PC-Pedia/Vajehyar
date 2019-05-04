using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace Vajehyar.Utility
{
    public class MyValueConverter : MarkupExtension, IValueConverter
    {
        private static MyValueConverter _converter = null;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (_converter == null) _converter = new MyValueConverter();
            return _converter;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (int)value + 2;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
