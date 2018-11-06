using PolygonDesigner.ViewLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolygonDesigner
{
    public class WpfDialogHandler : IDialogHandler
    {
        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        public Task<bool> ShowMessageBoxAsync(string message)
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                tcs.SetResult(MessageBox.Show(message) == MessageBoxResult.OK);
            });

            return tcs.Task;
        }

    }
}
