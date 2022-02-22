using BusinessLayer.Generales;
using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    public class ArticuloIngresadoEventArgs
    {
        public Articulo Articulo { get; set; }
        public bool IsToRemove { get; set; }

    }


    /// <summary>
    /// Lógica de interacción para DialogoIngresoArticulo.xaml
    /// </summary>
    public partial class DialogoIngresoArticulo : UserControl
    {
        public DialogoIngresoArticulo()
        {
            InitializeComponent();
            Calendario.Language = System.Windows.Markup.XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.ToString());
        }

        #region Variables
        private Articulo tempArticulo = null;
        private Articulo ArticuloSeleccionado = null;
        #region Propiedades


        public IEnumerable<object> Articulos
        {
            get { return (IEnumerable<object>)GetValue(ArticulosProperty); }
            set { SetValue(ArticulosProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Articulos.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArticulosProperty =
            DependencyProperty.Register("Articulos", typeof(IEnumerable<object>), typeof(DialogoIngresoArticulo), new PropertyMetadata(default(IEnumerable<object>)));




        public decimal Precio
        {
            get { return (decimal)GetValue(PrecioProperty); }
            set { SetValue(PrecioProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Precio.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrecioProperty =
            DependencyProperty.Register("Precio", typeof(decimal), typeof(DialogoIngresoArticulo), new PropertyMetadata(default(decimal)));



        public double Cantidad
        {
            get { return (double)GetValue(CantidadProperty); }
            set { SetValue(CantidadProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Cantidad.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CantidadProperty =
            DependencyProperty.Register("Cantidad", typeof(double), typeof(DialogoIngresoArticulo), new PropertyMetadata(default(double)));



        public int Envases
        {
            get { return (int)GetValue(EnvasesProperty); }
            set { SetValue(EnvasesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Envases.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnvasesProperty =
            DependencyProperty.Register("Envases", typeof(int), typeof(DialogoIngresoArticulo), new PropertyMetadata(default(int)));



        #endregion Propiedades

        #region Eventos
        public delegate void DelegadoArticuloIngresado(ArticuloIngresadoEventArgs e);
        public event DelegadoArticuloIngresado ArticuloIngresado;
        #endregion Eventos
        #endregion

        #region Funciones

        #endregion

        private void Box_Productos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArticuloSeleccionado = (Articulo)(sender as ComboBox).SelectedItem;
            if(ArticuloSeleccionado != null)
            {

                Box_Codigo.IsEnabled = false;
                Check_Retornable.IsEnabled = false;
                Check_Vencimiento.IsEnabled = false;


                Box_Codigo.Text = ArticuloSeleccionado.Codigo;
                Box_Precio.Text = ArticuloSeleccionado.Precio.ToString("C");
                Combo_Tipo_Cantidad.SelectedIndex = (int)ArticuloSeleccionado.TipoVenta;
                Check_Retornable.IsChecked = ArticuloSeleccionado.IsRetornable;

                if (ArticuloSeleccionado.Vencimientos.Keys.Max() != DateTime.MaxValue)
                {
                    Check_Vencimiento.IsChecked = true;
                }
            }
            else
            {
                Box_Codigo.IsEnabled = true;
                Check_Retornable.IsEnabled = true;
                Check_Vencimiento.IsEnabled = true;

                Cantidad = 0;
                Precio = 0;
                Combo_Tipo_Cantidad.SelectedIndex = 0;
                Check_Vencimiento.IsChecked = Check_Retornable.IsChecked = false;
            }


        }

        private void Box_Precio_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void Combo_Tipo_Cantidad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as ComboBox).SelectedIndex == 0)
            {
                Precio = 1;
            }
        }

        private void Boton_Aceptar_Click(object sender, RoutedEventArgs e)
        {
            if (Box_Cantidad.GetBindingExpression(TextBox.TextProperty).HasError
                || ((bool)Check_Vencimiento.IsChecked && Calendario.SelectedDate == null)
                || string.IsNullOrEmpty(Box_Productos.Text))
            {
                ArticuloIngresado?.Invoke(new ArticuloIngresadoEventArgs() { Articulo = null, IsToRemove = false });
                return;
            }
            else
            {
                var result = new Articulo()
                {
                    Codigo = Box_Codigo.Text,
                    Producto = Box_Productos.Text,
                    Precio = Precio,
                    Proveedor = Box_Proveedor.Text,
                    IsRetornable = (bool)Check_Retornable.IsChecked,
                    CantidadEnvases = Envases+(int)Cantidad,
                    TipoVenta = (TipoVenta)Combo_Tipo_Cantidad.SelectedIndex
                };
                result.AddStock(Math.Abs(Cantidad), Calendario.SelectedDate ?? DateTime.MaxValue);
                ArticuloIngresado?.Invoke(new ArticuloIngresadoEventArgs()
                {
                    Articulo = result,
                    IsToRemove = Cantidad < 0
                });
            }
        }
    }
}
