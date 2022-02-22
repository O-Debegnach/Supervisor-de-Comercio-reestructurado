using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VisualLayer.Resources.Classes.Commands
{
    public static class Open
    {

        //Comando para abrir la ventana de configuracion de la ticketera
        public static readonly RoutedUICommand ConfigTicketera = new RoutedUICommand
            (
                "Config. Ticketera",
                "OpenCfgTicketera",
                typeof(Open),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.T, ModifierKeys.Control | ModifierKeys.Alt)
                }
            );

        //Comando para abrir la ventana de logueo y registro
        public static readonly RoutedUICommand Login = new RoutedUICommand
            (
                "Login",
                "Login",
                typeof(Open)
            );
    }
}
