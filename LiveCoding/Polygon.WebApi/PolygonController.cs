using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Polygon.Core;

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

            // TODO: Clip source

            return Ok(new PolygonClipResponsePayload
            {
                SourcePolygon = request.SourcePolygon,
                ClipPolygon = request.ClipPolygon,
                ResultPolygon = PathMarkupConverter.Convert(source)
            });
        }
    }
}
