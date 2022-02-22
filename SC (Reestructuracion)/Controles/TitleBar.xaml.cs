using System;
using System.Collections.Generic;
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

namespace Controles
{
    /// <summary>
    /// Lógica de interacción para Titlebar.xaml
    /// </summary>
    public partial class Titlebar : UserControl
    {
        #region Variables
        private Window ParentWindow;

        #region Propiedades
        public int MyProperty { get; set; }

        public ResizeMode ResizeMode
        {
            get { return (ResizeMode)GetValue(ResizeModeProperty); }
            set 
            { 
                SetValue(ResizeModeProperty, value); 
                if(value == ResizeMode.CanMinimize)
                {
                    MaximizeRestoreButton.Visibility = Visibility.Collapsed;
                    MinimizeButton.Visibility = Visibility.Visible;
                }
                else if(value == ResizeMode.NoResize)
                {
                    MaximizeRestoreButton.Visibility = Visibility.Collapsed;
                    MinimizeButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    MaximizeRestoreButton.Visibility = Visibility.Visible;
                    MinimizeButton.Visibility = Visibility.Visible;

                }
            }
        }

        // Using a DependencyProperty as the backing store for ResizeMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResizeModeProperty =
            DependencyProperty.RegisterAttached("ResizeMode", typeof(ResizeMode), typeof(Titlebar), new PropertyMetadata(default(ResizeMode)));


        #endregion Propiedades
        #endregion Variables

        #region Funcione
        private Window GetParentWindow()
        {
            try
            {
                DependencyObject parent = Parent;
                parent = VisualTreeHelper.GetParent(parent);
                while (parent != null && !(parent is Window))
                {
                    parent = VisualTreeHelper.GetParent(parent);
                }

                return parent as Window;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Funciones

        public Titlebar()
        {
            InitializeComponent();
            ParentWindow = GetParentWindow();
            Binding b = new Binding()
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(Window), 1),
                Path = new PropertyPath("ResizeMode"),
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            this.SetBinding(ResizeModeProperty, b);
        }


        #region Eventos
        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ParentWindow == null) ParentWindow = GetParentWindow();

            if (ParentWindow.WindowState == WindowState.Maximized)
            {
                ParentWindow.WindowState = WindowState.Normal;
            }
            else
            {
                ParentWindow.WindowState = WindowState.Maximized;
            }
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow == null) ParentWindow = GetParentWindow();

            if (ParentWindow.WindowState == WindowState.Maximized)
            {
                ParentWindow.WindowState = WindowState.Normal;
            }
            else
            {
                ParentWindow.WindowState = WindowState.Maximized;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow == null) ParentWindow = GetParentWindow();
            ParentWindow.Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWindow == null) ParentWindow = GetParentWindow();

            ParentWindow.WindowState = WindowState.Minimized;
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ParentWindow == null) ParentWindow = GetParentWindow();

            ParentWindow.DragMove();
        }
        #endregion Eventos
    }
}