using System;
using Xunit;
using GeoJSON.Net.Geometry;
using WiM.Extensions;
using GeoJSON.Net.Converters;
using Newtonsoft.Json;

namespace Spatial.Test
{
    public class ExtensionsTest
    {
        [Fact]
        public void SqrContainsPoint()
        {
            Polygon poly = null;
            bool containsPoint;
            try
            {
                var geojsonPolygon = @"{""type"": ""Polygon"",""coordinates"": [[
                                        [-88.67717742919922,40.54185196245484],
                                        [-88.65022659301758,40.54185196245484],
                                        [-88.65022659301758,40.55450450121615],
                                        [-88.67717742919922,40.55450450121615],
                                        [-88.67717742919922,40.54185196245484]
                                        ]]}";
                
                poly = JsonConvert.DeserializeObject<Polygon>(geojsonPolygon, new GeometryConverter());

                containsPoint = poly.ContainsPoint(new Point(new Position(40.54967855709236, -88.67013931274414)));
                Assert.True(containsPoint);

                containsPoint = poly.ContainsPoint(new Point(new Position(40.54850462620186, -88.64748001098633)));
                Assert.False(containsPoint);

            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }
        public void ComplexContainsPoint()
        {
            Polygon poly = null;
            bool containsPoint;
            try
            {
                var geojsonPolygon = @"{""type"": ""Polygon"",""coordinates"": [[
                                            [-88.64215850830078,40.55293936825807],
                                        ]]}";

                poly = JsonConvert.DeserializeObject<Polygon>(geojsonPolygon, new GeometryConverter());

                containsPoint = poly.ContainsPoint(new Point(new Position(40.54967855709236, -88.67013931274414)));
                Assert.True(containsPoint);

                containsPoint = poly.ContainsPoint(new Point(new Position(40.54850462620186, -88.64748001098633)));
                Assert.False(containsPoint);

            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }
    }
}