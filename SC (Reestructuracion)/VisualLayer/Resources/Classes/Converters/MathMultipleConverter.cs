
using System;
using System.Globalization;
using System.Windows.Data;

namespace VisualLayer.Conversores
{
    public enum MathOperation { Add, Subtract, Multiply, Divide, Pow, Percentage, AddPercentage}
    public sealed class MathMultipleConverter : IMultiValueConverter
    {
        public MathOperation Operation { get; set; }

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.Length < 2 || value[0] == null || value[1] == null) return Binding.DoNothing;

            if (!double.TryParse(value[0].ToString(), NumberStyles.Any, culture, out double value1) || !double.TryParse(value[1].ToString(),NumberStyles.Any, culture, out double value2))
                return 0;

            switch (Operation)
            {
                default:
                    // (case MathOperation.Add:)
                    return value1 + value2;
                case MathOperation.Divide:
                    return value1 / value2;
                case MathOperation.Multiply:
                    return value1 * value2;
                case MathOperation.Subtract:
                    return value1 - value2;
                case MathOperation.Pow:
                    return Math.Pow(value1, value2);
                case MathOperation.Percentage:
                    return value1 * (value2 / 100);
                case MathOperation.AddPercentage:
                    return value1 + (value1 * (value2 / 100));
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}