using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisualLayer.Conversores
{
    [ValueConversion(typeof(int), typeof(bool))]
    public class IndexToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int p;
            if (parameter != null && int.TryParse(parameter.ToString(), out p))
            {
                return (int)value != p;
            }

            return value is int && (int)value != -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
