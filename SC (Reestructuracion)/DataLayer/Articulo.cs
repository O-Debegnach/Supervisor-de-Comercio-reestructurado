using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace DataLayer
{
    public enum Vencimiento { NoPosee = -1, NoVencido = 0, PorVencer = 1, Vencido = 2 }

    public enum TipoVenta : byte { PorUnidad = 2, PorPeso = 1, PorMonto = 0 }

    public enum AddStockResult
    {
        Success, 
        Error_InsufficientStock,
        Error_NoDate
    }
    [Serializable]
    public class Articulo : IEditableObject
    {
        #region Campos Privados
        private bool _editing;
        private Articulo temp_Articulo;
        private decimal _precio;
        private string _codigo;
        //private double _stockActual;
        private double _stockIdeal;
        private string _producto;
        private bool _isRetornable;
        private int _envases;
        private string _proveedor;
        private static readonly char separador = 'ð';
        #endregion Campos Privados

        #region Constructores
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="DataLayer.Articulo"/>, solicitando un codigo, nombre del producto, precio,
        /// el tipo de venta que posee, y el stock actual.
        /// </summary>
        /// <param name="codigo">Codigo que posee el <see cref="DataLayer.Articulo"/>, se usa al momento de realizar una busqueda</param>
        /// <param name="producto">Nombre del <see cref="DataLayer.Articulo"/></param>
        /// <param name="precio">Precio del <see cref="DataLayer.Articulo"/></param>
        /// <param name="tipoVenta">Tipo de Venta del <see cref="DataLayer.Articulo"/></param>
        /// <param name="stock_actual">Stock actual del <see cref="DataLayer.Articulo"/></param>
        public Articulo(string codigo, string producto, decimal precio, TipoVenta tipoVenta, double stock_actual)
        {
            Codigo = codigo;
            Producto = producto;
            Precio = precio;
            TipoVenta = tipoVenta;

            AddStock(stock_actual);
        }

        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="DataLayer.Articulo"/>, solicitando un codigo, nombre del producto, precio y
        /// el tipo de venta que posee.
        /// </summary>
        /// <param name="codigo">Codigo que posee el <see cref="DataLayer.Articulo"/>, se usa al momento de realizar una busqueda</param>
        /// <param name="producto">Nombre del <see cref="DataLayer.Articulo"/></param>
        /// <param name="precio">Precio del <see cref="DataLayer.Articulo"/></param>
        /// <param name="tipoVenta">Tipo de Venta del <see cref="DataLayer.Articulo"/></param>
        public Articulo(string codigo, string producto, decimal precio, TipoVenta tipoVenta)
        {
            Codigo = codigo;
            Producto = producto;
            Precio = precio;
            TipoVenta = tipoVenta;
        }

        /// <summary>
        /// Constructor vacio de la clase <see cref="Articulo"/>
        /// </summary>
        public Articulo() { }

        #endregion Constructores

        #region IPropertyChangedMembers
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChangedEvent;

        /// <summary>
        /// Notifica que la propiedad indicada en <paramref name="propertyName"/> cambio de valor
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que cambio de valor</param>
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEvent?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Metodos

        #region Metodos de Interfaces
        public void BeginEdit()
        {
            if (!_editing)
            {
                temp_Articulo = MemberwiseClone() as Articulo;
                _editing = true;
            }
        }

        public void EndEdit()
        {
            if (_editing)
            {
                temp_Articulo = null;
                _editing = false;
            }
        }

        public void CancelEdit()
        {
            if (_editing)
            {
                Producto = temp_Articulo.Producto;
                Precio = temp_Articulo.Precio;
                Stock_Ideal = temp_Articulo.Stock_Ideal;
                Codigo = temp_Articulo.Codigo;
            }
        }
        #endregion Metodos de Interfaces

        /// <summary>
        /// Verifica el estado de <see cref="Vencimiento"/> del <see cref="DataLayer.Articulo"/> 
        /// </summary>
        /// <returns><see cref="DataLayer.Vencimiento"/> Estado del vencimiento</returns>
        public Vencimiento CheckVencimiento()
        {
            if (Vencimientos == null || !(Vencimientos.Count > 0))
            {
                return Vencimiento.NoPosee;
            }

            if (ProximoVencimiento <= DateTime.Now)
            {
                return Vencimiento.Vencido;
            }
            else if (ProximoVencimiento <= DateTime.Now.AddDays(7))
            {
                return Vencimiento.PorVencer;
            }
            else
            {
                return Vencimiento.NoVencido;
            }
        }

        /// <summary>
        /// Compara la instancia actual con otra instancia de <see cref="DataLayer.Articulo"/>
        /// </summary>
        /// <param name="articulo"></param>
        /// <returns>Devuelve true si las propiedades <see cref="DataLayer.Articulo.Producto"/> son iguales, caso contrario devuelve false</returns>
        public bool Equals(Articulo articulo)
        {
            return Producto.Equals(articulo.Producto);
        }

        //Funciones a rehacer
        //public override string ToString()
        //{
        //    string tV;
        //    switch (TipoVenta)
        //    {
        //        case TipoVenta.PorUnidad:
        //            tV = "u";
        //            break;
        //        case TipoVenta.PorPeso:
        //            tV = "p";
        //            break;
        //        case TipoVenta.PorMonto:
        //            tV = "m";
        //            break;
        //        default:
        //            tV = "";
        //            break;
        //    }
        //    string s = Codigo + separador + Producto + separador + Precio.ToString(CultureInfo.InvariantCulture) + separador + (IsRetornable ? "true" : "false") +
        //        separador + CantidadEnvases.ToString(CultureInfo.InvariantCulture) + separador + tV + separador + _stockActual.ToString(CultureInfo.InvariantCulture) + separador + ProximoVencimiento.ToShortDateString();
        //    return s;
        //}

        //public static Articulo FromString(string s)
        //{
        //    string[] strings = s.Split(separador);
        //    if (strings.Length == 8)
        //    {
        //        TipoVenta tipoVenta;
        //        if (strings[5] == "u")
        //        {
        //            tipoVenta = TipoVenta.PorUnidad;
        //        }
        //        else if (strings[5] == "p")
        //        {
        //            tipoVenta = TipoVenta.PorPeso;
        //        }
        //        else if (strings[5] == "m")
        //        {
        //            tipoVenta = TipoVenta.PorMonto;
        //        }
        //        else
        //        {
        //            tipoVenta = TipoVenta.PorUnidad;
        //        }

        //        bool isRetornable = strings[3] == "true";

        //        DateTime.TryParse(strings[7], CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime fecha);

        //        Articulo art = new Articulo(strings[0], strings[1], Convert.ToDecimal(strings[2], CultureInfo.InvariantCulture), tipoVenta, Convert.ToDouble(strings[6], CultureInfo.InvariantCulture))
        //        {
        //            CantidadEnvases = Convert.ToInt32(strings[4], CultureInfo.InvariantCulture),
        //            IsRetornable = isRetornable
        //        };
        //        art.AddStock(Convert.ToDouble(strings[6], CultureInfo.InvariantCulture), fecha);
        //        return art;
        //    }
        //    return null;
        //}

        public AddStockResult AddStock(double cantidad, DateTime vencimiento)
        {
            if (Vencimientos == null)
            {
                Vencimientos = new Dictionary<DateTime, double>();
            }

            if (cantidad > 0)
            {
                if (Vencimientos.ContainsKey(vencimiento))
                {
                    Vencimientos[vencimiento] += cantidad;
                    if (Vencimientos[vencimiento] <= 0)
                    {
                        Vencimientos.Remove(vencimiento);
                    }
                }
                else
                {
                    Vencimientos.Add(vencimiento, cantidad);
                }

                if (IsRetornable) CantidadEnvases -= (int)cantidad;

                return AddStockResult.Success;
            }
            else if (cantidad < 0)
            {
                if (Vencimientos.ContainsKey(vencimiento))
                {
                    if (cantidad >= Vencimientos[vencimiento])
                    {
                        if (vencimiento != DateTime.MaxValue || Vencimientos.Count > 1)
                        {
                            Vencimientos.Remove(vencimiento);

                            if (IsRetornable) CantidadEnvases -= (int)cantidad;

                            return AddStockResult.Error_InsufficientStock;
                        }
                        else
                        {
                            Vencimientos[vencimiento] = 0;

                            if (IsRetornable) CantidadEnvases -= (int)cantidad;

                            return AddStockResult.Error_InsufficientStock;
                        }
                    }
                    else
                    {
                        Vencimientos[vencimiento] += cantidad;

                        if (IsRetornable) CantidadEnvases -= (int)cantidad;

                        return AddStockResult.Success;
                    }
                }
                else
                {
                    return AddStockResult.Error_NoDate;
                }
            }
            else
            {
                return AddStockResult.Success;
            }
        }

        public AddStockResult AddStock(Dictionary<DateTime, double> vencimientos)
        {
            int[] _results = new int[3];
            if (vencimientos == null)
            {
                return AddStockResult.Success;
            }

            foreach (DateTime date in vencimientos.Keys)
            {
                var r = AddStock(vencimientos[date], date);
                _results[(int)r]++;
            }

            if (_results[0] > _results[1] && _results[0] > _results[2]) return AddStockResult.Success;
            else if (_results[1] > _results[2]) return AddStockResult.Error_InsufficientStock;
            else return AddStockResult.Error_NoDate;
        }

        public AddStockResult AddStock(double cantidad)
        {
            if(Vencimientos.Count >= 1 && cantidad < 0)
            {
                cantidad *= -1;
                while (cantidad > Vencimientos[ProximoVencimiento])
                {
                    if(ProximoVencimiento == DateTime.MaxValue)
                    {
                        Vencimientos[ProximoVencimiento] = 0;
                        return AddStockResult.Error_InsufficientStock;
                    }

                    cantidad -= Vencimientos[ProximoVencimiento];
                    Vencimientos.Remove(ProximoVencimiento);

                    if(Vencimientos.Count == 0)
                    {
                        return AddStockResult.Error_InsufficientStock;
                    }
                }
                if(cantidad > 0)
                {
                    return AddStock(-cantidad, ProximoVencimiento);
                }
            }

            return AddStock(cantidad, DateTime.MaxValue);
        }

        #endregion Metodos

        #region Propiedades
        public Dictionary<DateTime, double> Vencimientos { get; set; }

        public Vencimiento EstadoVencimiento => CheckVencimiento();

        public TipoVenta TipoVenta { get; set; }

        public AddStockResult LastStockResult { get; private set; }
        public double Stock_Actual
        {
            get => Vencimientos.Values.Sum();
        }

        public DateTime ProximoVencimiento
        {
            get
            {
                if (Vencimientos != null && Vencimientos.Count > 0)
                {
                    return Vencimientos.Keys.Min();
                }
                else return DateTime.MaxValue;
            }
        }

        public decimal Precio
        {
            get => _precio;
            set
            {
                _precio = value;
                NotifyPropertyChanged(nameof(Precio));
            }
        }

        public string Codigo
        {
            get => _codigo;
            set
            {
                _codigo = value;
                NotifyPropertyChanged(nameof(Codigo));
            }
        }

        public double Stock_Ideal
        {
            get => _stockIdeal;
            set
            {
                _stockIdeal = value;
                NotifyPropertyChanged(nameof(Stock_Ideal));
            }
        }

        public string Proveedor
        {
            get => _proveedor;
            set
            {
                _proveedor = value;
                NotifyPropertyChanged(nameof(Proveedor));
            }
        }

        public int CantidadEnvases
        {
            get => _envases;
            set
            {
                _envases = value;
                NotifyPropertyChanged(nameof(CantidadEnvases));
            }
        }

        public string Producto
        {
            get => _producto;
            set
            {
                _producto = value;
                NotifyPropertyChanged(nameof(Producto));
            }

        }

        public bool IsRetornable
        {
            get => _isRetornable;
            set
            {
                _isRetornable = value;
                NotifyPropertyChanged(nameof(IsRetornable));
            }
        }

        #endregion Propiedades

        #region Operadores
        public static Articulo operator +(Articulo x, Articulo y)
        {
            if (y.Vencimientos != null && y.Vencimientos.Count > 0)
            {
                x.LastStockResult = x.AddStock(y.Vencimientos);
            }
            else
            {
                x.LastStockResult = x.AddStock(y.Stock_Actual);
            }
            x.CantidadEnvases += y.CantidadEnvases;

            return x;
        }

        public static Articulo operator -(Articulo x, Articulo y)
        {
            if (y.Vencimientos != null && y.Vencimientos.Count > 0)
            {
                foreach(DateTime d in y.Vencimientos.Keys)
                {
                    x.LastStockResult = x.AddStock(-y.Vencimientos[d], d);
                }
            }
            else
            {
                x.LastStockResult = x.AddStock(y.Stock_Actual);
            }
            x.CantidadEnvases += y.CantidadEnvases;

            return x;
        }
        #endregion Operadores

    }
}
