using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace VisualLayer.Conversores
{
    [ValueConversion(typeof(Thickness), typeof(Thickness))]
    class WorkSpaceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value is WindowState w)
            {
                if (w == WindowState.Maximized)
                {
                    Thickness margin = new Thickness();
                    Rect currentWorkArea = SystemParameters.WorkArea;
                    double currentScreenHeight = SystemParameters.PrimaryScreenHeight;
                    margin.Left = currentWorkArea.Left + 7;
                    margin.Top = currentWorkArea.Top + 7;
                    margin.Right = SystemParameters.PrimaryScreenWidth - currentWorkArea.Right + 7;
                    margin.Bottom = currentScreenHeight - currentWorkArea.Bottom + 6;

                    return margin;
                }
                return new Thickness(0);
            }
            else return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
