using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public enum TipoPago
    {
        Efectivo, Debito, Credito
    }

    public class VentaFinalizadaArgs
    {
        public TipoPago TipoPago { get; set; }
        public decimal Total { get; set; }
        public decimal Subtotal { get; set; }
        public TarjetaCredito Tarjeta { get; set; }
        public int Cuotas { get; set; }

    }
}
