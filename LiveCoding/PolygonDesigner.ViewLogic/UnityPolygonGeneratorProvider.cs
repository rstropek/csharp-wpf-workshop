using System;
using System.Collections.Generic;
using System.Text;
using Polygon.Core.Generators;
using Unity;

namespace PolygonDesigner.ViewLogic
{
    public class UnityPolygonGeneratorProvider : PolygonGeneratorProvider
    {
        private IUnityContainer Container;

        public UnityPolygonGeneratorProvider(IUnityContainer container) =>
            Container = container;

        public IEnumerable<PolygonGenerator> GetGenerators() =>
            Container.ResolveAll<PolygonGenerator>();
    }
}
