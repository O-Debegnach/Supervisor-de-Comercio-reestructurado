using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisualLayer.Dialogos
{

    /// <summary>
    /// Lógica de interacción para DoSellDialog.xaml
    /// </summary>
    public partial class DoSellDialog : UserControl, INotifyPropertyChanged
    {

        #region Propiedades
        #region Campos
        private Dictionary<string, decimal> _recargosDebito;
        private List<TarjetaCredito> _tarjetasCredito;
        private List<string> _mensajeCuotas;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<VentaFinalizadaArgs> VentaFinalizada;
        #endregion Campos
        #region DependencyProperty
        public decimal Total
        {
            get { return (decimal)GetValue(TotalProperty); }
            set 
            { 
                SetValue(TotalProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Total)));
            }
        }

        // Using a DependencyProperty as the backing store for Total.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total", typeof(decimal), typeof(DoSellDialog), new PropertyMetadata(default(decimal)));




        public decimal SubTotal
        {
            get { return (decimal)GetValue(SubTotalProperty); }
            set 
            { 
                SetValue(SubTotalProperty, value);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubTotal)));
            }
        }

        // Using a DependencyProperty as the backing store for SubTotal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubTotalProperty =
            DependencyProperty.Register("SubTotal", typeof(decimal), typeof(DoSellDialog), new PropertyMetadata(default(decimal)));

        #endregion DependencyProperty

        #region Props
        public Dictionary<string, decimal> RecargosDebito
        {
            get => _recargosDebito;
            set
            {
                _recargosDebito = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RecargosDebito)));
            }
        }
        public List<TarjetaCredito> TarjetasCredito
        {
            get => _tarjetasCredito;
            set
            {
                _tarjetasCredito = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TarjetasCredito)));
            }
        }

        public List<string> MensajesCuotas
        {
            get => _mensajeCuotas;
            set
            {
                _mensajeCuotas = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MensajesCuotas)));
            }
        }

        public int Cuotas { get; private set; }
        #endregion Props


        #endregion Propiedades

        public DoSellDialog(decimal subTotal, Dictionary<string, decimal> recargosDebito, List<TarjetaCredito> tarjetasCredito)
        {
            SubTotal = subTotal;
            RecargosDebito = recargosDebito ?? throw new ArgumentNullException(nameof(recargosDebito));
            TarjetasCredito = tarjetasCredito ?? throw new ArgumentNullException(nameof(tarjetasCredito));
            InitializeComponent();
        }

        private void ComboBoxVentas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox cb)
            {
                //Si el combobox cb es el de las tarjetas de credito
                if(cb.Name == nameof(cbTarjetasCredito))
                {

                    List<string> mensajes = new List<string>();
                    TarjetaCredito tc = cb.SelectedItem as TarjetaCredito;
                    foreach(int i in tc.InteresesCuotas.Keys)
                    {
                        var total = SubTotal + SubTotal * (tc.InteresesCuotas[i] / 100);
                        var cuota = total / i;
                        mensajes.Add($"{i,3}x{cuota,-12:$0.00}{total,15:($0.00)}");
                    }
                    MensajesCuotas = mensajes;
                }

                //Si el comboBox cb es el de las cuotas
                else if(cb.Name == nameof(cbCuotasCredito))
                {
                    if(cb.SelectedItem is string s)
                    {
                        int.TryParse(s.Split('x')[0], out int cuotas);
                        Cuotas = cuotas;
                        Total = SubTotal + (SubTotal * ((TarjetaCredito)cbTarjetasCredito.SelectedItem).InteresesCuotas[cuotas] / 100);
                    }
                }
            }
        }

        private void btFinalizarVenta_Click(object sender, RoutedEventArgs e)
        {
            if(lbTiposDePago.SelectedIndex == 0)
            {
                Total = SubTotal;
            }

            VentaFinalizada?.Invoke(new VentaFinalizadaArgs() 
            {
                TipoPago = (TipoPago)lbTiposDePago.SelectedIndex,
                Total = Total,
                Subtotal = SubTotal,
                Tarjeta = (TarjetaCredito)cbTarjetasCredito.SelectedItem,
                Cuotas = Cuotas
            });
        }
    }
}
