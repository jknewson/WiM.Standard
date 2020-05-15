using System;
using Xunit;
using GeoJSON.Net.Geometry;
using WIM.Extensions;
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
                                            [-88.6428451538086,40.54328690660496],
                                            [-88.63941192626953,40.54172151146236],
                                            [-88.63460540771484,40.54328690660496],
                                            [-88.6328887939453,40.550330732028456],
                                            [-88.63374710083008,40.55437407486699],
                                            [-88.63374710083008,40.55893884591851],
                                            [-88.6373519897461,40.562851259102054],
                                            [-88.64164352416992,40.56793705438169],
                                            [-88.64850997924805,40.56937143958841],
                                            [-88.66275787353516,40.56845865255947],
                                            [-88.67391586303711,40.567024247788396],
                                            [-88.6812973022461,40.561807971278185],
                                            [-88.68335723876953,40.55372193931024],
                                            [-88.68095397949219,40.54733067473287],
                                            [-88.66138458251953,40.54263466307469],
                                            [-88.65091323852539,40.54198241319326],
                                            [-88.64953994750977,40.54380869685629],
                                            [-88.65177154541016,40.545504487100736],
                                            [-88.65829467773438,40.54772199417569],
                                            [-88.65915298461914,40.55385236692972],
                                            [-88.6538314819336,40.55724339585951],
                                            [-88.64593505859375,40.5580259166068],
                                            [-88.64215850830078,40.55293936825807]
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
