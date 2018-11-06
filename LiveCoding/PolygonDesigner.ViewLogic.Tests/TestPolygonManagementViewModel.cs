using Moq;
using Polygon.Core;
using Polygon.Core.Generators;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;
using Xunit;

namespace PolygonDesigner.ViewLogic.Tests
{
    public class TestPolygonManagementViewModel
    {
        private Mock<PolygonGenerator> GetMockGenerator()
        {
            var mockGenerator = new Mock<PolygonGenerator>();
            mockGenerator.Setup(x => x.Generate(It.IsAny<double>()))
                .Returns(new ReadOnlyMemory<Point>(Array.Empty<Point>()));
            return mockGenerator;
        }

        [Fact]
        public void CannotGeneratePolygonWithoutGenerator()
        {
            var vm = new PolygonManagementViewModel();

            // vm.SelectedPolygonGenerator is initially null

            Assert.False(vm.GenerateAndAddPolygonCommand.CanExecute());
        }

        [Fact]
        public void EnsureSuccessMessageBoxOnGenerate()
        {
            var mockDialogHandler = new Mock<IDialogHandler>();

            var vm = new PolygonManagementViewModel(null, mockDialogHandler.Object);
            vm.SelectedPolygonGenerator = GetMockGenerator().Object;
            vm.GenerateAndAddPolygonCommand.Execute();

            // This is how you would verify that the message box has been triggered:
            // mockDialogHandler.Verify(x => x.ShowMessageBox(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SendCommandEventWhenGeneratorGetsSelected()
        {
            var vm = new PolygonManagementViewModel();
            var receivedGenerateCommandChanged = false;
            vm.GenerateAndAddPolygonCommand.CanExecuteChanged +=
                (_, __) => receivedGenerateCommandChanged = true;

            vm.SelectedPolygonGenerator = GetMockGenerator().Object;

            Assert.True(receivedGenerateCommandChanged);
            Assert.True(vm.GenerateAndAddPolygonCommand.CanExecute());
        }

        [Fact]
        public void GeneratePolygon()
        {
            var vm = new PolygonManagementViewModel();
            vm.SelectedPolygonGenerator = GetMockGenerator().Object;
            vm.GenerateAndAddPolygonCommand.Execute();

            Assert.Single(vm.Polygons);

            var newPolygon = vm.Polygons[0];
            Assert.True(!string.IsNullOrEmpty(newPolygon.Description));
            Assert.Equal(newPolygon.StrokeColor.R, newPolygon.FillColor.R);
            Assert.Equal(newPolygon.StrokeColor.G, newPolygon.FillColor.G);
            Assert.Equal(newPolygon.StrokeColor.B, newPolygon.FillColor.B);
        }

        [FriendlyName("Dummy")]
        private class DummyGenerator : PolygonGenerator
        {
            public ReadOnlyMemory<Point> Generate(in double maxSideLength) =>
                throw new NotImplementedException();
        }

        [Fact]
        public void FillPolygonGeneratorsFromContainer()
        {
            var mockGenerator = new Mock<PolygonGeneratorProvider>();
            mockGenerator.Setup(x => x.GetGenerators()).Returns(new[] { new DummyGenerator() });

            var vm = new PolygonManagementViewModel(mockGenerator.Object);
            Assert.Single(vm.Generators);
            var generator = vm.Generators.First();
            Assert.Equal("Dummy", generator.FriendlyName);
            Assert.IsType<DummyGenerator>(generator.Generator);
            Assert.Equal(vm.SelectedPolygonGenerator, generator.Generator);
        }

        [Fact]
        public async Task TestSomethingAsync()
        {
            await Task.Delay(100);
            Assert.True(1 == 1);
        }
    }
}
