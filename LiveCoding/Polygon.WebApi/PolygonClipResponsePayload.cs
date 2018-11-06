using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Polygon.WebApi
{
    public class PolygonClipResponsePayload
    {
        public string SourcePolygon { get; set; }
        public string ClipPolygon { get; set; }
        public string ResultPolygon { get; set; }
    }
}
