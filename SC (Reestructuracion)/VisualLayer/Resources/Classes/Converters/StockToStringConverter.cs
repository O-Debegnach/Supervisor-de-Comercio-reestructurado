using DataLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace VisualLayer.Conversores
{
    class StockToStringConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if(values.Length != 2)
            {
                throw new ArgumentException("Solo se aceptan 2 valores");
            }
            else
            {
                int d = -1, t = -1;
                for(int i = 0; i < 2; i++)
                {
                    if (values[i] is double) d = i;
                    if (values[i] is TipoVenta) t = i;
                }
                if(d == -1 || t == -1)
                {
                    return null;
                    //throw new ArgumentException($"Los tipos de datos pasados {values[0].GetType()} y {values[1].GetType()} son incorrectos");
                }
                
                switch ((TipoVenta)values[t])
                {
                    case TipoVenta.PorUnidad:
                        string s = (double)values[d] != 1 ? " unidades" : " unidad";
                        return ((double)values[d]).ToString(culture) + s;
                    case TipoVenta.PorPeso:
                        return ((double)values[d]).ToString(culture) + "Kg";
                    case TipoVenta.PorMonto:
                        return "$" + ((double)values[d]).ToString(culture);
                    default:
                        return d.ToString(culture);
                }
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
