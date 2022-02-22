using System;
using System.Collections.Generic;
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

namespace ControlLibrary
{
    /// <summary>
    /// Lógica de interacción para Titlebar.xaml
    /// </summary>
    public partial class Titlebar : UserControl
    {
        #region Variables
        private Window ParentWindow;

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
        }

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
    }
}