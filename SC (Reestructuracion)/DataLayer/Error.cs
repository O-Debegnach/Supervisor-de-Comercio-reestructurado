using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public enum ErrorCode
    {
        Ok, F1, D1, D2
    }

    public class Error
    {
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string Description { get; private set; }
        public ErrorCode ErrorCode { get; private set; }
        public MessageDialogState MessageDialogState { get; private set; }

        public static Error OK = new Error()
        {
            Title = "Listo",
            ErrorCode = ErrorCode.Ok,
            Message = "Operación realizada con exito",
            MessageDialogState = MessageDialogState.OK
        };

        public static Error F1 = new Error()
        {
            Title = "Error F1",
            ErrorCode = ErrorCode.F1,
            Message = "No se han cargado correctamente los datos en el formulario",
            Description = "Formulario mal cargado:\n\n" +
                          "Este error se presenta cuando algun dato del forulario no fue cargardo o estaba mal ingresado.\n" +
                          "Por ejemplo haber ingresado una letra en un campo que solo requeria numeros",
            MessageDialogState = MessageDialogState.Error
        };

        public static Error D1 = new Error()
        {
            Title = "Error D1",
            ErrorCode = ErrorCode.D1,
            Message = "Dado que la cantidad ingresada es mayor al stock disponible, solo se ha lo que estaba disponible",
            Description = "Falta de stock:\n\n" +
                          "Este error se presenta cuando se quiere descontar de un articulo o producto, " +
                          "una cantidad mayor al stock o cantidad de dicho articulo que registra el progama.\n" +
                          "Esto puede deberse a:\n" +
                          "  • Se retiraron unidades del stock pasarlo por el programa\n" +
                          "  • Se cargo mal la cantidad de productos que se deseaban descontar\n" +
                          "  • Se cargo mal la cantidad que se ingreso en algun momento\n" +
                          "  • Error al contabilizar el stock inicial",
            MessageDialogState = MessageDialogState.Warning
        };

        public static Error D2 = new Error()
        {
            Title = "Error D2",
            ErrorCode = ErrorCode.D2,
            Message = "No se han encontrado unidades del producto con la fecha de vencimiento seleccionada",
            Description = "No se encotro la fecha de vencimiento requerida:\n\n" +
                            "Este error se presenta cuando se desea retirar una cantidad determinada de articulos o productos" +
                            " que poseen vencimiento y la fecha de vencimiento requerida no se encuentra en stock.\n" +
                            "Esto puede deberse a:\n" +
                            "  • Error al contabilizar stock\n" +
                            "  • Se ingreso mal la fecha de la que se deseaba descontar\n" +
                            "  • Se realizaron extracciones de unidades sin registrarla en el programa",
            MessageDialogState = MessageDialogState.Error
        };
        
    }
}
