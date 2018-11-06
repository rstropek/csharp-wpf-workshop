using System;
using System.Collections.Generic;
using System.Text;

namespace Polygon.Core
{
    public class AreaCalculationProgress
    {
        public double ProgressPercentage { get; }
        public double AreaIntermediateResult { get; }

        public AreaCalculationProgress(double progressPercentage, double areaIntermediateResult)
        {
            ProgressPercentage = progressPercentage;
            AreaIntermediateResult = areaIntermediateResult;
        }
    }
}
