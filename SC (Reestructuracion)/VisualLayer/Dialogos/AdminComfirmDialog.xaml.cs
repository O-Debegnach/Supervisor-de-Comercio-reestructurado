using BusinessLayer.Login;
using DataLayer;
using MaterialDesignThemes.Wpf;
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
using VisualLayer.Ventanas;

namespace VisualLayer.Dialogos
{
    public enum DialogAction
    {
        Confirmar, Cancelar
    }

    public class AdminConfirmEventArgs
    {
        public DialogAction DialogAction;
        public Usuario Usuario;    }
    /// <summary>
    /// Lógica de interacción para AdminComfirmDialog.xaml
    /// </summary>
    public partial class AdminComfirmDialog : UserControl
    {
        public List<Usuario> Usuarios { get; set; }
        public string UserName { get; set; }

        public delegate void AdminConfirmedDelegate(AdminConfirmEventArgs e);
        public event AdminConfirmedDelegate AdminConfirmed;


        public AdminComfirmDialog(IEnumerable<Usuario> usuarios)
        {
            InitializeComponent();
            Usuarios = usuarios.Where(user => user.Administrador).ToList();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Name.Equals(btConfirmar.Name))
            {
                if (!UserBox.GetBindingExpression(TextBox.TextProperty).HasError)
                {
                    Usuario verif = LoginHelper.confirmLogin(Usuarios, UserName, passBox.Password);
                    if (verif != null)
                    {
                        AdminConfirmed?.Invoke(new AdminConfirmEventArgs() { Usuario = verif, DialogAction = DialogAction.Confirmar });
                    }
                    else
                    {
                        lblError.Visibility = Visibility.Visible;
                    }
                }
            }
            else if (((Button)sender).Name.Equals(btCancelar.Name))
            {
                AdminConfirmed?.Invoke(new AdminConfirmEventArgs() { Usuario = null, DialogAction = DialogAction.Cancelar });
            }
        }
    }
}
