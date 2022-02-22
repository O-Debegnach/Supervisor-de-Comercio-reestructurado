using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Login
{
    public class UserSigInArgs
    {
        public Usuario Usuario { get; set; }
        public int Error { get; set; }
    }
    public static class LoginHelper
    {
        public static Usuario confirmLogin(IEnumerable<Usuario> usuarios, string usuario, string password)
        {
            var user = usuarios.ToList().Find(x => x.Nombre.Equals(usuario));

            if (user != null && user.Contraseña.Equals(password, StringComparison.Ordinal))
            {
                return user;
            }
            return null;
        }
    }
}
