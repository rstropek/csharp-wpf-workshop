using Polygon.Core.Generators;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolygonDesigner.ViewLogic
{
    public interface PolygonGeneratorProvider
    {
        IEnumerable<PolygonGenerator> GetGenerators();
    }
}
