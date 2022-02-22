using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class TarjetaCredito
    {
        public string Nombre { get; set; }
        public Dictionary<int, decimal> InteresesCuotas { get; set; }
    }
}
