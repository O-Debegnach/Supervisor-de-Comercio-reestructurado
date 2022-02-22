using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class Oferta
    {
        public Dictionary<Articulo, double> ProductosAfectados { get; set; }
        public decimal PrecioPorOferta { get; set; }
        public decimal DescuentoPorOferta
        {
            get
            {
                var precioTotal = ProductosAfectados.Keys.Sum(x => x.Precio);
                return PrecioPorOferta - precioTotal;
            }
        }
        public bool SoloCantidadExacta { get; set; }

        //Crear funcion para detectar si la oferta es aplicable y cuantas veces
    }
}
