using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace VisualLayer.Conversores
{
    [ValueConversion(typeof(Brush), typeof(Brush), ParameterType = typeof(int))]
    class ColorDarknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((value ?? false) is Brush brush )
            {
                if(brush is SolidColorBrush solid)
                {
                    Color color = solid.Color;
                    if(color.R + color.G + color.B > 255)
                    {
                        return new SolidColorBrush(Colors.Black);
                    }
                    return new SolidColorBrush(Colors.White);
                }
                if (brush is GradientBrush gradient)
                {
                    int prom = 0;
                    foreach(GradientStop stop in gradient.GradientStops)
                    {
                        prom += stop.Color.R + stop.Color.G + stop.Color.B;
                    }
                    prom /= gradient.GradientStops.Count;
                    if(prom > 255)
                    {
                        return new SolidColorBrush(Colors.Black);
                    }
                    return new SolidColorBrush(Colors.White);
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
