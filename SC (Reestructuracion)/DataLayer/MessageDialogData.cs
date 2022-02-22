using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public enum MessageDialogState
    {
        OK, Error, Warning, Info
    }

    public enum MessageDialogButtons
    {
        YesNo, OKCancel, AcceptCancel, Nothing
    }

    public enum MessageDialogResult
    {
        OK, Cancel, None
    }
}
