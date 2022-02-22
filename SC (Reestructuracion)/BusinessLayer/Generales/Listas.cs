using DataLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Generales
{
    public static class Listas
    {
        public static int PorEstadoVencimiento(Articulo x, Articulo y)
        {
            if (x.EstadoVencimiento > y.EstadoVencimiento)
            {
                return -1;
            }
            else if (x.EstadoVencimiento == y.EstadoVencimiento)
            {
                return x.Producto.CompareTo(y.Producto);
            }
            else return 1;
        }

        public static int UsuarioPorNombre(Usuario x, Usuario y)
        {
            return x.Nombre.CompareTo(y.Nombre);
        }


        public static IEnumerable<ArticuloVenta> AddItemToSell(IEnumerable<ArticuloVenta> list, ArticuloVenta item)
        {
            var lista = list?.ToList() ?? new List<ArticuloVenta>();
            var index = lista.FindIndex(x => x == item);
            if (index != -1)
            {
                lista[index] += item;
            }
            else
            {
                lista.Add(item);
            }
            return lista;
        }


        public static IEnumerable<Articulo> DescontarStock(IEnumerable<Articulo> articulos, IEnumerable<ArticuloVenta> descuento)
        {
            var arts = articulos.ToList();
            var desc = descuento.ToList();

            foreach(ArticuloVenta av in desc)
            {
                int index = arts.FindIndex(x => x == av.ArticuloOrigen);
                if (index != -1) 
                    arts[index].AddStock(-av.Cantidad);
            }

            return arts;
        }

        /// <summary>
        /// Busca un elemento en <see cref="IEnumerable{T}"/> list comparando con <see cref="Predicate{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="match"></param>
        /// <returns>Si encuentra el elemento retorna <see cref="int"/> indice en el que se encuentra, caso contrario devuelve -1</returns>
        public static int FindIndex<T>(this IEnumerable<T> list, Predicate<T> match)
        {
            for(int i=0; i<list.Count(); i++)
            {
                if (match(list.ElementAt(i)))
                {
                    return i;
                }
            }
            return -1;
        }


        public static void FiltrarEnvases(this ICollectionView list)
        {
            list.Filter = f => ((Articulo)f).IsRetornable;
        }
    }

}
