using PolygonDesigner.ViewLogic;
using System.ComponentModel;
using System.Windows;

namespace PolygonDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(PolygonManagementViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
