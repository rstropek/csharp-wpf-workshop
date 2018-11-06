using Polygon.Core;
using Polygon.Core.Generators;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolygonDesigner.ViewLogic
{
    public class PolygonManagementViewModel : BindableBase
    {
        private IDialogHandler DialogHandler { get; }
        private PolygonClipper Clipper { get; }

        public const double MaxSideLength = 400d;

        public PolygonManagementViewModel(PolygonGeneratorProvider generatorProvider = null, 
            IDialogHandler dialogHandler = null,
            PolygonClipper clipper = null)
        {
            Polygons = new ObservableCollection<Polygon>();

            DialogHandler = dialogHandler;
            Clipper = clipper;

            GenerateAndAddPolygonCommand = new DelegateCommand(
                GenerateAndAddPolygon,
                () => SelectedPolygonGenerator != null);

            ClipPolygonsCommand = new DelegateCommand(
                ClipPolygons,
                () => Polygons.Count == 2 && Clipper != null);
            Polygons.CollectionChanged += (_, __) => ClipPolygonsCommand.RaiseCanExecuteChanged();

            if (generatorProvider != null)
            {
                Generators = GetGeneratorsFromContainer(generatorProvider);
                SelectedPolygonGenerator = Generators.LastOrDefault()?.Generator;
            }
        }

        private void ClipPolygons()
        {
            if (Polygons.Count != 2)
            {
                throw new InvalidOperationException("Clipping is only possible for two and exactly two polygons");
            }

            if (Clipper == null)
            {
                throw new InvalidOperationException("No polygon clipper set");
            }

            var resultPolygon = Clipper.GetIntersectedPolygon(Polygons[0].Points, Polygons[1].Points);

            if (resultPolygon.IsEmpty)
            {
                DialogHandler?.ShowMessageBox("No overlap");
            }
            else
            {
                Polygons.Clear();
                var color = GenerateRandomColor();
                Polygons.Add(new Polygon
                {
                    Description = "Clipped Polygon",
                    Points = resultPolygon,
                    StrokeColor = color,
                    FillColor = Color.FromArgb(128, color)
                });
            }
        }

        private IEnumerable<GeneratorInfo> GetGeneratorsFromContainer(PolygonGeneratorProvider generatorProvider) =>
            generatorProvider.GetGenerators()
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
                GenerateAndAddPolygonCommand.RaiseCanExecuteChanged();
            }
        }

        private Polygon SelectedPolygonValue;
        public Polygon SelectedPolygon
        {
            get { return SelectedPolygonValue; }
            set { SetProperty(ref SelectedPolygonValue, value); }
        }

        public DelegateCommand GenerateAndAddPolygonCommand { get; }

        public DelegateCommand ClipPolygonsCommand { get; }

        private void GenerateAndAddPolygon()
        {
            if (SelectedPolygonGenerator == null)
            {
                throw new InvalidOperationException($"No polygon generator selected");
            }

            var polygon = SelectedPolygonGenerator.Generate(MaxSideLength);
            var color = GenerateRandomColor();
            Polygons.Add(new Polygon
            {
                Points = polygon,
                Description = $"Auto-generated Polygon at {DateTime.Now:D}",
                StrokeColor = color,
                FillColor = Color.FromArgb(128, color)
            });

            // This is how you would show a message box:
            // DialogHandler?.ShowMessageBox("Polygon has been added");
        }

        private async Task GenerateAndAddPolygonAsync()
        {
            if (SelectedPolygonGenerator == null)
            {
                throw new InvalidOperationException($"No polygon generator selected");
            }

            var polygon = SelectedPolygonGenerator.Generate(MaxSideLength);
            var color = GenerateRandomColor();
            Polygons.Add(new Polygon
            {
                Points = polygon,
                Description = $"Auto-generated Polygon at {DateTime.Now:D}",
                StrokeColor = color,
                FillColor = Color.FromArgb(128, color)
            });

            if (DialogHandler != null)
            {
                await DialogHandler.ShowMessageBoxAsync("Polygon has been added");
            }
        }

        private Color GenerateRandomColor()
        {
            var random = new Random();
            return Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
        }
    }
}
