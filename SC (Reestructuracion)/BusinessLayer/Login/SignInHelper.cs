using DataLayer;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls;
using System.Windows.Media;

namespace BusinessLayer.Login
{
    public class SignInHelper
    {
        PasswordBox passwordBox, confirmPasswordBox;
        TextBox usernameBox;
        public Brush NormalBrush { get; set; }
        public Brush ErrorBrush { get; set; } = Brushes.Red;
        public bool IsAdmin { get; set; } = false;
        public SignInHelper(PasswordBox passwordBox, PasswordBox confirmPasswordBox, TextBox usernameBox)
        {
            this.passwordBox = passwordBox;
            this.confirmPasswordBox = confirmPasswordBox;
            this.usernameBox = usernameBox;

            passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
            confirmPasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is PasswordBox p)
            {
                if (p.Password.Length < 4)
                {
                    HintAssist.SetHelperText(p, "Debe tener al menos 4 caracteres.");
                    TextFieldAssist.SetUnderlineBrush(p, ErrorBrush);
                }
                else
                {
                    HintAssist.SetHelperText(p, "");
                    TextFieldAssist.SetUnderlineBrush(p, NormalBrush);
                }
            }
        }

        public bool CheckPassword()
        {
            if (string.IsNullOrWhiteSpace(usernameBox.Text))
            {
                HintAssist.SetHelperText(passwordBox, "El usuario no puede estar en blanco.");
                usernameBox.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }

            bool pswrdOk;
            if (passwordBox.Password.Length < 4)
            {
                HintAssist.SetHelperText(passwordBox, "Debe tener al menos 4 caracteres.");
                TextFieldAssist.SetUnderlineBrush(passwordBox, ErrorBrush);
                passwordBox.BorderBrush = ErrorBrush;
                pswrdOk = false;
            }
            else
            {
                HintAssist.SetHelperText(passwordBox, "");
                TextFieldAssist.SetUnderlineBrush(passwordBox, NormalBrush);
                passwordBox.BorderBrush = NormalBrush;
                pswrdOk = true;
            }

            bool confirmOk;
            if (confirmPasswordBox.Password.Length < 4)
            {
                HintAssist.SetHelperText(confirmPasswordBox, "Debe tener al menos 4 caracteres.");
                TextFieldAssist.SetUnderlineBrush(confirmPasswordBox, ErrorBrush);
                confirmPasswordBox.BorderBrush = ErrorBrush;
                confirmOk = false;
            }
            else if (!confirmPasswordBox.Password.Equals(passwordBox.Password, System.StringComparison.Ordinal))
            {
                HintAssist.SetHelperText(confirmPasswordBox, "Las Contraseñas no coinciden.");
                TextFieldAssist.SetUnderlineBrush(confirmPasswordBox, ErrorBrush);
                confirmPasswordBox.BorderBrush = ErrorBrush;
                confirmOk = false;
            }
            else
            {
                HintAssist.SetHelperText(confirmPasswordBox, "");
                TextFieldAssist.SetUnderlineBrush(confirmPasswordBox, NormalBrush);
                confirmPasswordBox.BorderBrush = NormalBrush;
                confirmOk = true;
            }

            return pswrdOk & confirmOk;
        }

        public Usuario DoSignIn()
        {
            if (usernameBox.GetBindingExpression(TextBox.TextProperty).HasError || !CheckPassword()) return null;

            return new Usuario(usernameBox.Text, passwordBox.Password, IsAdmin);
        }
    }
}
