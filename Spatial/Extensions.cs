using System;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace WiM.Spatial
{
    public static class Extensions
    {
        //Adapted from Turfjs https://github.com/Turfjs/turf-inside/blob/master/index.js#L65
        public static Boolean ContainsPoint(this MultiPolygon polygon, Point point)
        {
            try
            {
                var poly = polygon.Coordinates.Select(p => p.Coordinates.ToList().Select(p2=>p2.Coordinates.ToList()).ToList()).ToList();
                var containsPoint = false;
                var i = 0;
                while (i < polygon.Coordinates.Count() && !containsPoint)
                {
                    containsPoint = poly[i].ContainsPoint(point.Coordinates);
                    i++;
                }//next

                return containsPoint;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static Boolean ContainsPoint(this Polygon polygon, Point point)
        {
            var poly = polygon.Coordinates.Select(p => p.Coordinates.ToList()).ToList();
            return poly.ContainsPoint(point.Coordinates);

        }
        public static Boolean ContainsPoint(this List<List<IPosition>> poly, IPosition pt)
        {
            try
            {
                var containsPoint = false;
                // check if it is in the outer ring first
                if (poly[0].ContainsPoint(pt))
                {
                    var i = 1;
                    var inHole = false;
                    while (i < poly.Count() && !inHole)
                    {
                        if (poly[i].ContainsPoint(pt)) inHole = true;
                        i++;
                    }//next
                    if (!inHole) containsPoint = true;
                }//endif

                return containsPoint;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static Boolean ContainsPoint(this List<IPosition> ring, IPosition pt)
        {
            var isInside = false;
            var j = ring.Count() - 1;
            for (var i = 0; i < ring.Count(); j = i++)
            {
                var xi = ring[i].Latitude;
                var yi = ring[i].Longitude;
                var xj = ring[j].Latitude;
                var yj = ring[j].Longitude;

                var intersect = ((yi > pt.Longitude) != (yj > pt.Longitude)) &&
                    (pt.Latitude < (xj - xi) * (pt.Longitude - yi) / (yj - yi) + xi);
                if (intersect) isInside = !isInside;
            }
            return isInside;
        }

    }
}
