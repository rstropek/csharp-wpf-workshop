using Moq;
using Polygon.Core;
using Polygon.Core.Generators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PolygonDesigner.ViewLogic.Tests
{
    public class TestPolygonClippingViewLogic
    {
        [Fact]
        public void ClipNotEnabledIfPolygonsCountIsNotEqualTwo()
        {
            var mockGenerator = new Mock<PolygonGenerator>();
            var mockProvider = new Mock<PolygonGeneratorProvider>();
            mockProvider.Setup(x => x.GetGenerators()).Returns(new[] { mockGenerator.Object });
            var mockClipper = new Mock<PolygonClipper>();

            var vm = new PolygonManagementViewModel(mockProvider.Object, null, mockClipper.Object);

            // Polygons initially empty --> command must be disabled
            Assert.False(vm.ClipPolygonsCommand.CanExecute());

            vm.GenerateAndAddPolygonCommand.Execute();
            Assert.False(vm.ClipPolygonsCommand.CanExecute());

            var receivedExecuteChanged = false;
            vm.ClipPolygonsCommand.CanExecuteChanged += (_, __) => receivedExecuteChanged = true;
            vm.GenerateAndAddPolygonCommand.Execute();
            Assert.True(vm.ClipPolygonsCommand.CanExecute());
            Assert.True(receivedExecuteChanged);

            receivedExecuteChanged = false;
            vm.GenerateAndAddPolygonCommand.Execute();
            Assert.False(vm.ClipPolygonsCommand.CanExecute());
            Assert.True(receivedExecuteChanged);
        }

        [Fact]
        public void ClipReplacesExistingPolygons()
        {
            var mockGenerator = new Mock<PolygonGenerator>();
            var mockProvider = new Mock<PolygonGeneratorProvider>();
            mockProvider.Setup(x => x.GetGenerators()).Returns(new[] { mockGenerator.Object });
            var mockClipper = new Mock<PolygonClipper>();
            mockClipper.Setup(x => x.GetIntersectedPolygon(It.IsAny<ReadOnlyMemory<Point>>(), It.IsAny<ReadOnlyMemory<Point>>()))
                .Returns(new[] { new Point(0d, 0d) });

            var vm = new PolygonManagementViewModel(mockProvider.Object, null, mockClipper.Object);

            vm.GenerateAndAddPolygonCommand.Execute();
            vm.GenerateAndAddPolygonCommand.Execute();

            vm.ClipPolygonsCommand.Execute();
            Assert.Single(vm.Polygons);
        }
    }
}
