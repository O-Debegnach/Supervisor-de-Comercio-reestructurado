using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    [Serializable]
    public class Venta
    {
        public IEnumerable<ArticuloVenta> ArticulosVendidos { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        public Usuario Responsable { get; set; }
        public TipoPago TipoPago { get; set; }

        public TarjetaCredito Tarjeta { get; set; }
        public int Cuotas { get; set; }



        public string SaveString()
        {
            string result = $"Fecha: {Fecha.ToLongDateString()}\t" +
                            $"Hora: {Fecha.ToLongTimeString()}\n" +
                            $"Responsable o Usuario Registrado: {Responsable.Nombre}\n" +
                            $"Tipo de pago: {(TipoPago == TipoPago.Efectivo ? "Efectivo" : TipoPago == TipoPago.Debito ? "Debito" : "Credito")}\n" +
                            $"{(TipoPago == TipoPago.Credito ? $"Tarjeta: {Tarjeta.Nombre}   Cuotas: {Cuotas}x{Total/Cuotas:C}   Interes: {Tarjeta.InteresesCuotas[Cuotas]}%" : "")}\n" +
                            $"Articulos vendidos:\n" +
                            $"\t{"Articulo",15} | {"Cantidad",10} | {"Precio Unitario",20} | {"Precio Total",15:C} |\n";

            var renglonExtra = "";
            foreach(ArticuloVenta art in ArticulosVendidos)
            {
                string nombre = art.ArticuloOrigen.Producto;
                if(nombre.Length > 15)
                {
                    renglonExtra = nombre.Substring(15);
                    nombre = nombre.Substring(0, 15);
                }

                result += $"\t{nombre,-15} | {art.Cantidad,10} | {art.ArticuloOrigen.Precio, 20:C} | {art.ArticuloOrigen.Precio*(decimal)art.Cantidad,15:C} |\n";

                while (!string.IsNullOrWhiteSpace(renglonExtra))
                {
                    result += $"\t{(renglonExtra.Length > 15 ? renglonExtra.Substring(0,15) : renglonExtra), -15} | {"",10} | {"",20} | {"",15} |\n";
                    if (renglonExtra.Length > 15)
                    {
                        renglonExtra = renglonExtra.Substring(15);
                    }
                    else renglonExtra = string.Empty;
                }
            }
            result += $"\n\nSubtotal: {Subtotal:C}";
            result += $"\nTotal: {Total:C}";
            return result;
        }
    }
}
