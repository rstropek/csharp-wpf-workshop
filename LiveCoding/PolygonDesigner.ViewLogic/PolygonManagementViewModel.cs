using Polygon.Core.Generators;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using Unity;

namespace PolygonDesigner.ViewLogic
{
    public class PolygonManagementViewModel : BindableBase
    {
        public PolygonManagementViewModel(IUnityContainer container = null)
        {
            Polygons = new ObservableCollection<Polygon>();

            if (container != null)
            {
                Generators = GetGeneratorsFromContainer(container);
                SelectedPolygonGenerator = Generators.LastOrDefault()?.Generator;
            }
        }

        private IEnumerable<GeneratorInfo> GetGeneratorsFromContainer(IUnityContainer container) =>
            container.ResolveAll<PolygonGenerator>()
                .Select(g => new GeneratorInfo(this,
                    (g.GetType()
                        .GetCustomAttributes(typeof(FriendlyNameAttribute), true)
                        .FirstOrDefault() as FriendlyNameAttribute)?.FriendlyName ?? g.GetType().Name,
                    g));

        public ObservableCollection<Polygon> Polygons { get; }

        public IEnumerable<GeneratorInfo> Generators { get; }

        private PolygonGenerator SelectedPolygonGeneratorValue;
        public PolygonGenerator SelectedPolygonGenerator
        {
            get { return SelectedPolygonGeneratorValue; }
            set
            {
                SetProperty(ref SelectedPolygonGeneratorValue, value);
                //GenerateAndAddPolygonCommand.RaiseCanExecuteChanged();
            }
        }

        private Polygon SelectedPolygonValue;
        public Polygon SelectedPolygon
        {
            get { return SelectedPolygonValue; }
            set { SetProperty(ref SelectedPolygonValue, value); }
        }

    }
}
