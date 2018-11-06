using Microsoft.AspNetCore.Mvc;
using Polygon.Core.Generators;
using System.Linq;

namespace Polygon.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolygonGeneratorController : ControllerBase
    {
        [HttpGet]
        public ActionResult Get() =>
            Ok(new[]
            {
                typeof(TrianglePolygonGenerator),
                typeof(SquarePolygonGenerator),
                typeof(RandomPolygonGenerator)
            }.Select(t => t.FullName));
    }
}
