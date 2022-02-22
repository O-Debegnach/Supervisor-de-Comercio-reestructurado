using DataLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Generales
{
    public static class Archivos
    {
        public static void GuardarVentas(Venta venta, string directorio, ulong IDVenta)
        {
            string rutaFinal = directorio + $@"\{DateTime.Now:dd-MM-yy}";
            if (!Directory.Exists(rutaFinal))
            {
                Directory.CreateDirectory(rutaFinal);
            }

            File.WriteAllText(rutaFinal+$@"\{IDVenta}.txt", venta.SaveString());
        }
    }
}
