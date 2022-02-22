using DataLayer;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using VisualLayer.Ventanas;
using BusinessLayer.Archivos;
using BusinessLayer.Generales;
using VisualLayer.Properties;
using System.Linq;
using System.Timers;
using VisualLayer.Dialogos;
using System.Windows.Data;
using VisualLayer.Controles;
using System.Windows.Controls;
using System.ComponentModel;
using Res = VisualLayer.Resources.Strings.VisualStrings;
using System.Collections.ObjectModel;

namespace VisualLayer
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Variables
        #region Campos Privados
        private ObservableCollection<Articulo> _articulos;
        private Timer AutoSaveTimer;
        private Usuario UsuarioLogueado;
        #endregion Campos Privados

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion INotifyPropertyChanged

        #region Ventanas y Dialogos
        private ConfiguracionTicketera cfgTicketera;
        private LoginWindow loginWindow;
        #endregion Ventanas y Dialogos

        #region Propiedades

        public ICollectionView ItemListEnvases
        {
            get
            {
                var r = new CollectionViewSource { Source = Articulos }.View;
                r.Filter = f => ((Articulo)f).IsRetornable;
                return r;
            }
        }

        public ICollectionView ItemListArticulos
        {
            get => new CollectionViewSource { Source = Articulos }.View;
        }

        public decimal TotalDeVenta
        {
            get
            {
                if (ArticulosVenta != null)
                {
                    return (decimal)ArticulosVenta?.Sum(x => x.PrecioTotal);
                }
                else return 0m;
            }
        }

        public List<ArticuloVenta> ArticulosVenta { get; set; }

        public ObservableCollection<Articulo> Articulos
        {
            get
            {
                if (_articulos == null) _articulos = new ObservableCollection<Articulo>();
                return _articulos;
            }

            set
            {
                _articulos = value;
                NotifyPropertyChanged(nameof(Articulos));
            }
        }

        public List<Usuario> Usuarios { get; set; }
        #endregion
        #endregion Variables


#if DEBUG
        Random r = new Random();

        DateTime RandomDate()
        {
            return DateTime.Now.AddDays(-15 + r.Next(30));
        }
#endif

        public MainWindow()
        {
            #region Inicializar Configuraciones
            if (string.IsNullOrWhiteSpace(Settings.Default.RutaBase))
            {
                var s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                s += "\\V&D\\Supervisor de Comercio";
                Settings.Default.RutaBase = s;
                Settings.Default.Save();
            }
            #endregion Inicializar Configuraciones

            AutoSaveTimer = new Timer(Settings.Default.TiempoAutoSave * 1000)
            {
                AutoReset = true,
                Enabled = true
            };
            AutoSaveTimer.Elapsed += AutoSaveTimer_Elapsed;

            #region Login
            Usuarios = FileManager.Load<Usuario>(Settings.Default.FullSavePath + Settings.Default.UsersFile).ToList();

            loginWindow = new LoginWindow(Usuarios);
            loginWindow.UserLogued += LoginWindow_UserLogued;
            loginWindow.Closed += LoginWindow_Closed;
            loginWindow.ShowDialog();
            #endregion Login

            InitializeComponent();

#if DEBUG
            List<Articulo> arts = new List<Articulo>();
            for(int i = 0; i<100; i++)
            {
                var a = new Articulo(i.ToString(), string.Format("Art {0}", i), i, (TipoVenta)r.Next(3));
                a.AddStock(i + 1, RandomDate().Date);
                a.IsRetornable = r.Next() % 2 == 0;
                if (a.IsRetornable) a.CantidadEnvases = r.Next(100);
                arts.Add(a);
            }
            FileManager.Save(Settings.Default.FullSavePath + Settings.Default.StockFile, arts);

#endif

            #region Articulos
            
            Articulos = new ObservableCollection<Articulo>(FileManager.Load<Articulo>(Settings.Default.FullSavePath + Settings.Default.StockFile));
            Articulos.CollectionChanged += delegate { ItemListArticulos.Refresh(); ItemListEnvases.Refresh(); };
            NotifyPropertyChanged(nameof(ItemListArticulos));
            NotifyPropertyChanged(nameof(ItemListEnvases));
            #endregion Articulos

        }

        #region Manejo de Stock

        private void BtIngresoPedido_Click(object sender, RoutedEventArgs e)
        {
            DialogoIngresoArticulo DialogoIngresoPedidos = new DialogoIngresoArticulo()
            {
                Articulos = Articulos
            };
            DialogoIngresoPedidos.ArticuloIngresado += DialogoIngresoPedidos_ArticuloIngresado;
            DialogHostStock.DialogContent = DialogoIngresoPedidos;
        }

        private void DialogoIngresoPedidos_ArticuloIngresado(ArticuloIngresadoEventArgs e)
        {

            if(e.Articulo == null)
            {
                DialogHostStock.DialogContent = new MessageDialog(Error.F1);
                return;
            }

            int index = Articulos.FindIndex(x => x.Producto.Equals(e.Articulo.Producto));
            if (index >= 0)
            {
                var prevStock = Articulos[index].Stock_Actual;
                if (e.IsToRemove)
                {
                    Articulos[index] -= e.Articulo;
                }
                else
                {
                    Articulos[index] += e.Articulo;
                }

                switch (Articulos[index].LastStockResult)
                {
                    case AddStockResult.Success:
                        DialogHostStock.DialogContent = new MessageDialog(Error.OK);
                        break;

                    case AddStockResult.Error_InsufficientStock:
                        DialogHostStock.DialogContent = new MessageDialog(Error.D1);
                        break;
                    case AddStockResult.Error_NoDate:
                        DialogHostStock.DialogContent = new MessageDialog(Error.D2);
                        break;
                }
            }
            else
            {
                Articulos.Add(e.Articulo); 
                DialogHostStock.DialogContent = new MessageDialog(Error.OK);
            }

            DGEnvases.ItemsSource = null;
            DGEnvases.ItemsSource = Articulos.SkipWhile(x => x.IsRetornable == false);
        }

        #endregion Manejo de Stock

        #region Manejo de Archivos
        private void AutoSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            AutoSaveTimer.Interval = Settings.Default.TiempoAutoSave * 1000;
            FileManager.Save(Settings.Default.FullSavePath + Settings.Default.StockFile, Articulos);
        }
        #endregion Manejo de Archivos

        #region Login
        private void LoginWindow_Closed(object sender, EventArgs e)
        {
            if(UsuarioLogueado == null)
            {
                Close();
            }
        }

        private void LoginWindow_UserLogued(LoginEventArgs e)
        {
            string s = $"{(e.Usuario.Administrador? Res.loginAdm : Res.loginUsr)} {e.Usuario.Nombre}";
            this.Title = s;
            this.UpdateLayout();
            UsuarioLogueado = e.Usuario;

            if (e.IsNewUser)
            {
                Usuarios.Add(e.Usuario);
                Usuarios.Sort(Listas.UsuarioPorNombre);
                FileManager.Save(Settings.Default.FullSavePath + Settings.Default.UsersFile, Usuarios);
            }
        }
        #endregion Login

        #region Menu Desplegable
        private void ListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (toggleButton != null) toggleButton.IsChecked = false;
            toggleAdministrar.IsChecked = false;
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            menuList.SelectedIndex = -1;
            toggleButton.IsChecked = false;
        }
        #endregion Menu Desplegable

        #region Configuracion Ticketera
        private void ConfigTicketera_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ConfigTicketera_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (cfgTicketera == null)
            {
                cfgTicketera = new ConfiguracionTicketera();
                cfgTicketera.Closed += CfgTicketera_Closed;
                cfgTicketera.Show();
            }
            else
            {
                cfgTicketera.Focus();
            }
        }

        private void CfgTicketera_Closed(object sender, EventArgs e)
        {
            cfgTicketera = null;
        }

        #endregion Confiracion Ticketera

        #region Ventas
        private void SearchBar_SearchDone(SeachResult e)
        {
            dgProductosVenta.ItemsSource = null;
            dgProductosVenta.ItemsSource = e.Result;
        }

        private void Ventas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                if (sender is SearchBar)
                {
                    if (dgProductosVenta.Items.Count == 1)
                    {
                        ArticulosVenta = (List<ArticuloVenta>)Listas.AddItemToSell(ArticulosVenta, new ArticuloVenta() { ArticuloOrigen = (Articulo)dgProductosVenta.Items[0], Cantidad = 1 });
                        SearchBarVentas.Clear();
                    }
                }
                else if (sender is DataGrid d)
                {
                    if (d.SelectedItem != null)
                    {
                        ArticulosVenta = (List<ArticuloVenta>)Listas.AddItemToSell(ArticulosVenta, new ArticuloVenta() { ArticuloOrigen = (Articulo)d.SelectedItem, Cantidad = 1 });
                        e.Handled = true;
                    }
                }
                NotifyPropertyChanged(nameof(TotalDeVenta));
                dgVentas.ItemsSource = null;
                dgVentas.ItemsSource = ArticulosVenta;
            } 
        }

        private void Ventas_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid d && d.Name.Equals(nameof(dgProductosVenta)))
            {
                if (d.SelectedItem != null)
                {
                    var articuloVenta = new ArticuloVenta() { ArticuloOrigen = (Articulo)d.SelectedItem, Cantidad = 1 };
                    articuloVenta.PropertyChanged += ArticuloVenta_PropertyChanged;
                    ArticulosVenta = (List<ArticuloVenta>)Listas.AddItemToSell(ArticulosVenta, articuloVenta);
                    e.Handled = true;
                }
            }
            NotifyPropertyChanged(nameof(TotalDeVenta));
            dgVentas.ItemsSource = null;
            dgVentas.ItemsSource = ArticulosVenta;
        }

        private void ArticuloVenta_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TotalDeVenta));
        }

        private void dgVentas_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            NotifyPropertyChanged(nameof(TotalDeVenta));
        }

        private void Ventas_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button bt)
            {
                if (bt.Name == nameof(btTerminarVenta))
                {
                    var rd = new Dictionary<string, decimal>() { { "General", 5 } };
                    var tc = new List<TarjetaCredito>() { new TarjetaCredito() { Nombre = "Todas",
                                                          InteresesCuotas = new Dictionary<int, decimal> { { 1, 5 }, { 3, 15 } } } };
                    var dialogo = new DoSellDialog(TotalDeVenta, rd, tc)
                    {
                        Width = 350
                    };

                    dialogo.VentaFinalizada += DialogoVenta_VentaFinalizada;
                    DialogHostVentas.DialogContent = dialogo;
                }
            }
        }

        private void DialogoVenta_VentaFinalizada(VentaFinalizadaArgs e)
        {
            Venta venta = new Venta()
            {
                Fecha = DateTime.Now,
                ArticulosVendidos = ArticulosVenta,
                Responsable = UsuarioLogueado,
                TipoPago = e.TipoPago,
                Total = e.Total,
                Subtotal = e.Subtotal,
                Tarjeta = e.Tarjeta,
                Cuotas = e.Cuotas
            };
            Archivos.GuardarVentas(venta, Settings.Default.RegistroVentasCompleto, Settings.Default.IDVenta);

            var dialog = new MessageDialog("¿Desea Imprimir el ticket?", MessageDialogState.Info, MessageDialogButtons.YesNo);
            dialog.OnClosing += DialogImpresionTicket_OnClosing;
            DialogHostVentas.DialogContent = dialog;

            Listas.DescontarStock(Articulos, ArticulosVenta);
            ArticulosVenta.Clear();

            NotifyPropertyChanged(nameof(TotalDeVenta));
            dgVentas.ItemsSource = null;
            dgVentas.ItemsSource = ArticulosVenta;

            Settings.Default.IDVenta++;
            Settings.Default.Save();
        }

        private void DialogImpresionTicket_OnClosing(MessageDialogResult obj)
        {
            if(obj == MessageDialogResult.OK)
            {
                //Insertar fragmento de impresion de ticket
            }
        }

        #endregion Ventas

        private void ContextMenuStock_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem item)
            {
                if(item.Header.ToString() == "Producto")
                {
                    var sortDirection = DGStock.Columns[0].SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    BusinessLayer.Generales.Controles.SortDataGrid(DGStock, 0, sortDirection);
                }
                else if (item.Header.ToString() == "Codigo")
                {
                    var sortDirection = DGStock.Columns[1].SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    BusinessLayer.Generales.Controles.SortDataGrid(DGStock, 1, sortDirection);
                }
                else if (item.Header.ToString() == "Cantidad")
                {
                    var sortDirection = DGStock.Columns[2].SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    BusinessLayer.Generales.Controles.SortDataGrid(DGStock, 2, sortDirection);
                }
                else if (item.Header.ToString() == "Precio")
                {
                    var sortDirection = DGStock.Columns[3].SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    BusinessLayer.Generales.Controles.SortDataGrid(DGStock, 3, sortDirection);
                }
                else if (item.Header.ToString() == "Vencimiento")
                {
                    var sortDirection = DGStock.Columns[4].SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    BusinessLayer.Generales.Controles.SortDataGrid(DGStock, 4, sortDirection);
                }
            }
        }

        private void ContextMenuItemStock_Checked(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem item)
            {
                foreach (DataGridColumn c in DGStock.Columns)
                {
                    if (c.Header.ToString() == item.Header.ToString())
                    {
                        c.Visibility = item.IsChecked ? Visibility.Visible : Visibility.Hidden;
                        break;
                    }
                }
            }
        }

        private void DGStock_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Delete)
            {
                e.Handled = true;
                var items = DGStock.SelectedItems;
                var msg = $"¿Desea eleminar {(items.Count > 1 ? "los articulos seleccionados" : "el articulo seleccionado")}?";
                var dialog = new MessageDialog(msg, MessageDialogState.Info, MessageDialogButtons.YesNo);
                dialog.OnClosing += DialogoBorrarProducto_OnClosing;
                DialogHostStock.DialogContent = dialog;
                DialogHostStock.IsOpen = true;
            }
        }

        private void DialogoBorrarProducto_OnClosing(MessageDialogResult obj)
        {
            if(obj == MessageDialogResult.OK)
            {
                foreach(object o in DGStock.SelectedItems)
                {
                    DGStock.Items.Remove(o);
                }
            }
        }

        private void SearchBarStock_SearchDone(SeachResult e)
        {
            if (e.Result.Count() > 0)
            {
                DGStock.SelectedItem = e.Result.First();
                DGStock.ScrollIntoView(DGStock.SelectedItem);
            }
        }

        private void DGStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is DataGrid d)
            {
                if(d.Name == nameof(DGStock))
                {
                    if (d.SelectedItem != null && (d.SelectedItem as Articulo).IsRetornable)
                    {
                        DGEnvases.SelectedItem = d.SelectedItem;
                        DGEnvases.ScrollIntoView(d.SelectedItem);
                    }
                    else DGEnvases.SelectedItem = null;
                }
            }
        }
    }
}
