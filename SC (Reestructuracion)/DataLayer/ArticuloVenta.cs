using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    [Serializable]
    public class ArticuloVenta:INotifyPropertyChanged
    {
        private double _cantidad;
        public Articulo ArticuloOrigen { get; set; }
        public double Cantidad
        {
            get => _cantidad;
            set
            {
                if(value != _cantidad)
                {
                    _cantidad = value;
                    NotifyPropertyChanged(nameof(Cantidad));
                    NotifyPropertyChanged(nameof(PrecioTotal));
                }
            }
        }

        public decimal PrecioTotal { get => ArticuloOrigen.Precio * (ArticuloOrigen.TipoVenta == TipoVenta.PorMonto ? (decimal)Cantidad/1000 :(decimal)Cantidad); }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public override bool Equals(object obj)
        {
            if(obj is ArticuloVenta art)
            {
                return ArticuloOrigen == art.ArticuloOrigen;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static ArticuloVenta operator +(ArticuloVenta x, ArticuloVenta y)
        {
            x.Cantidad += y.Cantidad;
            return x;
        }
        public static ArticuloVenta operator -(ArticuloVenta x, ArticuloVenta y)
        {
            x.Cantidad -= y.Cantidad;
            return x;
        }

        public static bool operator ==(ArticuloVenta x, ArticuloVenta y) => x.ArticuloOrigen.Producto.Equals(y.ArticuloOrigen.Producto);
        public static bool operator !=(ArticuloVenta x, ArticuloVenta y) => !x.ArticuloOrigen.Producto.Equals(y.ArticuloOrigen.Producto);
    }
}
