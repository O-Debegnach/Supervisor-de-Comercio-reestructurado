using BusinessLayer.Archivos;
using BusinessLayer.Login;
using DataLayer;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisualLayer.Properties;

namespace VisualLayer.Ventanas
{

    /// <summary>
    /// Dato devuelto al loguearse o registrarse un empleado en LoginWindow
    /// </summary>
    public class LoginEventArgs
    {
        public bool IsNewUser { get; set; }
        public Usuario Usuario { get; set; }
    }

    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    /// 
    public partial class LoginWindow : Window
    {
        #region Variables
        #region Eventos y delegados
        public delegate void UserLoguedDelegate(LoginEventArgs e);
        public event UserLoguedDelegate UserLogued;

        private readonly SignInHelper SignInHelper;
        private string _userName;
        #endregion Eventos y delegados

        #region Campos Privados
        #endregion Campos Privados

        #region Propiedades
        private List<Usuario> Usuarios { get; }
        public string UserName {
            get
            {
                return _userName;
            }
            set
            {
                if(value != _userName)
                {
                    _userName = value;
                }
            } 
        }

        public bool IsAdmin { get; set; }
        #endregion Propiedades
        #endregion Variables
        public LoginWindow(IEnumerable<Usuario> usuarios)
        {
            InitializeComponent();
            Usuarios = new List<Usuario>();

            SignInHelper = new SignInHelper(regPassword, regConfirmPassword, boxUserReg)
            {
                NormalBrush = BorderBrush
            };

            if (usuarios != null && usuarios.Count() > 0)
            {
                Usuarios = usuarios.ToList();
            }
            else
            {
                gdLogin.Visibility = Visibility.Collapsed;
                gdRegistro.Visibility = Visibility.Visible;
                adminCheckBox.IsChecked = true;
                adminCheckBox.IsEnabled = false;
            }

            Usuarios.Add(Usuario.DebugUser);
        }


        #region Login
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!boxUsuario.GetBindingExpression(TextBox.TextProperty).HasError)
            {
                Usuario verif = LoginHelper.confirmLogin(Usuarios, UserName, boxContraseña.Password);
                _ = UserName;
                if (verif != null)
                {
                    UserLogued?.Invoke(new LoginEventArgs() { Usuario = verif, IsNewUser = false });
                    Close();
                }
                else
                {
                    lblError.Visibility = Visibility.Visible;
                }
            }
        }

        #endregion Login

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            Usuario u = SignInHelper.DoSignIn();
            if (u != null)
            {
                UserLogued?.Invoke(new LoginEventArgs() { IsNewUser = true, Usuario = u });
                Close();
            }
        }

        private void LabelButtons_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var t = sender as Label;
            if (t.Name == LabelRegistro.Name)
            {
                gdLogin.Visibility = Visibility.Collapsed;
                LabelIngresoExistente.Visibility = gdRegistro.Visibility = Visibility.Visible;
                boxUsuario.Text = boxContraseña.Password = "";
            }
            else if(t.Name == LabelIngresoExistente.Name || t.Name == LabelVolver.Name)
            {
                LabelIngresoExistente.Visibility = gdPassword.Visibility = gdRegistro.Visibility = Visibility.Collapsed;
                gdLogin.Visibility = Visibility.Visible;
                boxUserCP.Text = boxCP.Password = boxCCP.Password = null;
                boxUserReg.Text = regPassword.Password = regConfirmPassword.Password = null;
            }
            else if(t.Name == LabelOlvidoContraseña.Name)
            {
                gdLogin.Visibility = Visibility.Collapsed;
                gdPassword.Visibility = Visibility.Visible;
                boxUsuario.Text = boxContraseña.Password = "";
            }
        }

        private void CambiarContraseña_Click(object sender, RoutedEventArgs e)
        {
            SignInHelper cp = new SignInHelper(boxCP, boxCCP, boxUserCP);
            if (cp.CheckPassword())
            {
                var dialogo = new Dialogos.AdminComfirmDialog(Usuarios);
                dialogo.AdminConfirmed += Dialogo_AdminConfirmed;
                DHCambioContraseña.DialogContent = dialogo;
                DHCambioContraseña.IsOpen = true;
            }
        }

        private void Dialogo_AdminConfirmed(Dialogos.AdminConfirmEventArgs e)
        {
            if(e.DialogAction == Dialogos.DialogAction.Confirmar)
            {
                if(e.Usuario != null)
                {
                    var user = Usuarios.Find(x => x.Nombre.Equals(boxUserCP.Text));
                    if(user != null)
                    {
                        user.Contraseña = boxCP.Password;
                        DHCambioContraseña.IsOpen = false;
                        gdLogin.Visibility = Visibility.Visible;
                        gdPassword.Visibility = Visibility.Collapsed;
                        FileManager.Save(Settings.Default.FullSavePath + Settings.Default.UsersFile, Usuarios);
                    }
                }
                else
                {
                    DHCambioContraseña.IsOpen = true;
                }
            }
        }
    }
}
