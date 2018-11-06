using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PolygonDesigner.ViewLogic
{
    public interface IDialogHandler
    {
        void ShowMessageBox(string message);

        Task<bool> ShowMessageBoxAsync(string message);
    }
}
