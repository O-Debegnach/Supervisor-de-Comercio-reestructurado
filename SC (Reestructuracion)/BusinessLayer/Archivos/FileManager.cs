using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Archivos
{
    public abstract class FileManager
    {
        public static bool MooveFile(string originPath, string finalPath)
        {
            try
            {
                Directory.Move(originPath, finalPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<T> Load<T>(string ruta, Predicate<T> condicion)
        {
            try
            {
                using (Stream st = File.OpenRead(ruta))
                {
                    List<T> list = new List<T>();
                    BinaryFormatter binfor = new BinaryFormatter();
                    long t = st.Length;
                    while (st.Position < t)
                    {
                        T a = (T)binfor.Deserialize(st);
                        if (condicion != null && condicion(a))
                        {
                            list.Add(a);
                        }
                        else if (condicion == null)
                        {
                            list.Add(a);
                        }
                    }
                    st.Close();
                    return list;
                }
            }
            catch (Exception)
            {
                return Enumerable.Empty<T>();
            }
        }

        public static IEnumerable<T> Load<T>(string ruta)
        {
            return Load<T>(ruta, null);
        }

        public static int Save<T>(string ruta, IEnumerable<T> lista, Predicate<T> condicion)
        {
            try
            {
                if (File.Exists(ruta))
                {
                    File.Delete(ruta);
                }

                string dir = ruta.Substring(0, ruta.LastIndexOf('\\'));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }


                using (Stream st = File.Open(ruta, FileMode.Create, FileAccess.Write))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    foreach (T obj in lista)
                    {
                        if (condicion != null && condicion(obj))
                        {
                            formatter.Serialize(st, obj);
                        }
                        else if (condicion == null)
                        {
                            formatter.Serialize(st, obj);
                        }
                    }
                    st.Close();
                    //File.Delete(temp);
                    return 0;
                }
            }
            catch (Exception)
            {
                return 1;
            }

        }

        public static int Save<T>(string ruta, IEnumerable<T> lista)
        {
            return Save(ruta, lista, null);
        }
    }
}
