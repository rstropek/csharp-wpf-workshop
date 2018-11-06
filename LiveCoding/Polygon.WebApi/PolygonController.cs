using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polygon.Core;
using Polygon.Core.Generators;

namespace Polygon.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolygonController : ControllerBase
    {
        [HttpPost]
        [Route("clip")]
        public ActionResult Post([FromBody] PolygonClipRequestPayload request)
        {
            if (string.IsNullOrEmpty(request.SourcePolygon))
            {
                return BadRequest("Missing source polygon");
            }

            if (string.IsNullOrEmpty(request.ClipPolygon))
            {
                return BadRequest("Missing clipping polygon");
            }

            var source = PathMarkupConverter.Convert(request.SourcePolygon);
            var clip = PathMarkupConverter.Convert(request.ClipPolygon);

            var result = new SutherlandHodgman().GetIntersectedPolygon(source, clip);

            return Ok(new PolygonResultPayload
            {
                ResultPolygon = PathMarkupConverter.Convert(result)
            });
        }

        [HttpPost]
        [Route("generate")]
        public ActionResult Post([FromBody] PolygonGenerateRequestPayload request)
        {
            if (string.IsNullOrEmpty(request.Generator))
            {
                return BadRequest("Missing clipping polygon");
            }

            var type = typeof(PolygonGenerator).Assembly.DefinedTypes
                .FirstOrDefault(t => t.FullName == request.Generator);
            if (type == null)
            {
                return BadRequest("Invalid generator name");
            }

            var generator = (PolygonGenerator)Activator.CreateInstance(type);
            var polygon = generator.Generate(request.MaxSideLength);

            return Ok(new PolygonResultPayload
            {
                ResultPolygon = PathMarkupConverter.Convert(polygon)
            });
        }
    }
}
