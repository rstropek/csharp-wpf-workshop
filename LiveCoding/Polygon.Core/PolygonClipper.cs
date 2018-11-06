using System;

namespace Polygon.Core
{
    /// <summary>
    /// Clips a subject polygon against a clip polygon
    /// </summary>
    public interface PolygonClipper
    {
        ReadOnlyMemory<Point> GetIntersectedPolygon(in ReadOnlyMemory<Point> subjectPoly, in ReadOnlyMemory<Point> clipPoly);

        /// <summary>
        /// This clips the subject polygon against the clip polygon (gets the intersection of the two polygons)
        /// </summary>
        /// <param name="subjectPoly">Can be concave or convex</param>
        /// <param name="clipPoly">Must be convex</param>
        /// <returns>The intersection of the two polygons (or null)</returns>
        ReadOnlyMemory<Point> GetIntersectedPolygon(in ReadOnlySpan<Point> subjectPoly, in ReadOnlySpan<Point> clipPoly);
    }
}
