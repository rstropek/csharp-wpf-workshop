using Polygon.Core;
using Polygon.Core.Generators;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PolygonDesigner.ViewLogic
{
    public class PolygonManagementViewModel : BindableBase
    {
        private IDialogHandler DialogHandler { get; }
        private PolygonClipper Clipper { get; }
        private AreaCalculator AreaCalculator { get; }

        public const double MaxSideLength = 400d;

        public PolygonManagementViewModel(PolygonGeneratorProvider generatorProvider = null, 
            IDialogHandler dialogHandler = null,
            PolygonClipper clipper = null,
            AreaCalculator areaCalculator = null)
        {
            Polygons = new ObservableCollection<Polygon>();

            DialogHandler = dialogHandler;
            Clipper = clipper;
            AreaCalculator = areaCalculator;

            GenerateAndAddPolygonCommand = new DelegateCommand(
                GenerateAndAddPolygon,
                () => SelectedPolygonGenerator != null);

            ClipPolygonsCommand = new DelegateCommand(
                ClipPolygons,
                () => Polygons.Count == 2 && Clipper != null);
            Polygons.CollectionChanged += (_, __) => ClipPolygonsCommand.RaiseCanExecuteChanged();

            CalculateAreaForSelectedPolygonCommand = new DelegateCommand(
                CalculateAreaForSelectedPolygon,
                () => SelectedPolygon != null && !IsCalculatingArea && AreaCalculator != null);

            CancelAreaCalculationCommand = new DelegateCommand(
                () => CancelSource.Cancel(),
                () => IsCalculatingArea);

            if (generatorProvider != null)
            {
                Generators = GetGeneratorsFromContainer(generatorProvider);
                SelectedPolygonGenerator = Generators.LastOrDefault()?.Generator;
            }

            MouseCommand = new DelegateCommand<Polygon>((x) => SelectedPolygon = x);
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
            set
            {
                SetProperty(ref SelectedPolygonValue, value);
                CalculateAreaForSelectedPolygonCommand.RaiseCanExecuteChanged();
            }
        }

        private bool IsCalculatingAreaValue;
        public bool IsCalculatingArea
        {
            get { return IsCalculatingAreaValue; }
            private set
            {
                SetProperty(ref IsCalculatingAreaValue, value);
                CalculateAreaForSelectedPolygonCommand.RaiseCanExecuteChanged();
                CancelAreaCalculationCommand.RaiseCanExecuteChanged();
            }
        }

        private double ProgressValue;
        public double Progress
        {
            get { return ProgressValue; }
            private set { SetProperty(ref ProgressValue, value); }
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

        public DelegateCommand CalculateAreaForSelectedPolygonCommand { get; }

        public DelegateCommand CancelAreaCalculationCommand { get; }

        private double? AreaValue;
        public double? Area
        {
            get { return AreaValue; }
            private set
            {
                SetProperty(ref AreaValue, value);
                RaisePropertyChanged(nameof(AreaAvailable));
            }
        }

        public bool AreaAvailable => Area.HasValue;

        private async void CalculateAreaForSelectedPolygon()
        {
            if (IsCalculatingArea)
            {
                throw new InvalidOperationException("Calculation already running");
            }

            if (AreaCalculator == null)
            {
                throw new InvalidOperationException("No Area Calculator defined");
            }

            if (SelectedPolygon == null)
            {
                throw new InvalidOperationException("No polygon selected");
            }

            IsCalculatingArea = true;

            using (CancelSource = new CancellationTokenSource())
            {
                try
                {
                    Area = await AreaCalculator.CalculateAreaAsync(
                        SelectedPolygon.Points,
                        CancelSource.Token,
                        new Progress<AreaCalculationProgress>(p =>
                        {
                            Progress = p.ProgressPercentage;
                            Area = p.AreaIntermediateResult;
                        }));
                }
                catch (OperationCanceledException)
                {
                    // Add logging later...
                }
            }

            IsCalculatingArea = false;
        }

        private CancellationTokenSource CancelSource;

        public DelegateCommand<Polygon> MouseCommand { get; }
    }
}
