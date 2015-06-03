//------------------------------------------------------------------------------
//----- FeatureResource -----------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2013 WiM - USGS

//    authors:  Jeremy K. Newson USGS Wisconsin Internet Mapping
//              
//  
//   purpose:   Feature resources.
//              Equivalent to the model in MVC.
//
//discussion:   Used for support in serializing for ESRI/GeoJson featureCollections
//
//     

#region Comments
// 02.08.13 - jkn - Created
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WiM.Resources.Spatial
{
    public class FeatureWrapper
    {
        public string name { get; set; }
        public FeatureCollectionBase feature { get; set; }
    }

    #region Base classes

    public abstract class ExtentBase
    {
        #region Properties      
        public double xMin { get; set; }
        public double xMax { get; set; }
        public double yMin { get; set; }
        public double yMax { get; set; }
        public double zMin { get; set; }
        public double zMax { get; set; }
        #endregion
        #region Base Constructors
        public ExtentBase()
        { }
        #endregion
    }//end Extentbase

    [XmlInclude(typeof(EsriPolygon))]
    [XmlInclude(typeof(EsriPoint))]
    [XmlInclude(typeof(EsriPolyline))]
    public abstract class GeometryBase
    {
        #region Base Properties
        
        #endregion
        
        #region Base Constructors
        public GeometryBase()
        { }
        #endregion
        public abstract List<double> getBoundingBox();
    }//end GeometryBase

    public abstract class FeatureBase
    {
        #region Base Properties
        public abstract string type { get; set; }
        public GeometryBase geometry { get; set; }
        public Attributes attributes { get; set; }
        #endregion
        #region Base Constructors
        public FeatureBase()
        { }
        #endregion
        #region Base Methods
        protected abstract Boolean FromJson(JObject jobj);
        #endregion
        
    }//end FeatureBase

    public abstract class FeatureCollectionBase
    { //represents an interface comparison of GeoJson's FeatureCollection and ESRI's FeatureRecordset
        #region Base Properties
        [XmlIgnore]
        public abstract CoordianteReferenceSystemBase crs { get; set; }
        private List<FeatureBase> _features = new List<FeatureBase>();

        [XmlArrayItem("feature")]
        public List<FeatureBase> features 
        { 
            get { return _features; } 
        }
        public abstract Boolean addFeature(FeatureBase feature);
        #endregion
        #region Base Constructors
        public FeatureCollectionBase()
        { }
        #endregion
        #region Base Methods
        #endregion
       
    }//end FeatureCollectionBase

    public abstract class CoordianteReferenceSystemBase
    {
        #region Base Properties
		//If an Esri well-known ID is below 32767, it corresponds to the EPSG ID
        [XmlIgnore]
        public abstract object srObject { get; set; }
	    #endregion
        #region Base Constructors
        public CoordianteReferenceSystemBase()
        { }
        #endregion
        #region Base Methods
        
        #endregion
  
    }//end CoordinateReferenceSystemBase

    public class Attributes
    {
        [XmlElement("objectid")]
        [JsonProperty("objectid")]
        public int OBJECTID { get; set; }
        [XmlElement("hydroid")]
        [JsonProperty("hydroid")]
        public int HydroID { get; set; }
        [XmlElement("drainid")]
        [JsonProperty("drainid")]
        public int DrainID { get; set; }
        [XmlElement("hucid")]
        [JsonProperty("hucid")]
        public string HUCID { get; set; }

    }
    #endregion

    #region Esri-based RecordSet

    [XmlInclude(typeof(SpatialReference))]
    [XmlInclude(typeof(EsriFeature))]
    public class EsriFeatureRecordSet:FeatureCollectionBase
    {
        #region Fields & Properties
        public string geometryType { get; set; }
        [XmlElement(ElementName = "spatialReference")]
        [JsonProperty(PropertyName="spatialReference")]
        public override CoordianteReferenceSystemBase crs { get; set; }  
        #endregion
        #region Constructors
        public EsriFeatureRecordSet()
        { }
        public EsriFeatureRecordSet(Int32 wkid, string geomType)
            :this()
        {
            this.crs = new SpatialReference(wkid);
            this.geometryType = geomType;
        }
        public EsriFeatureRecordSet(FeatureBase feature, Int32 wkid)
            : this(wkid, feature.type)
        {
            features.Add(feature);
        }
        public EsriFeatureRecordSet(List<FeatureBase> features, Int32 wkid, string geomType)
            : this(wkid, geomType)
        {
            features.ForEach(f=>addFeature(f));
        }
        #endregion
        #region Explicit operator
        //used for casting
        public static explicit operator EsriFeatureRecordSet(FeatureCollection featureRS)
        {
            throw new NotImplementedException();
        }//end FeatureCollection
        #endregion
        #region Methods
        public override Boolean addFeature(FeatureBase feature)
        {
            try
            {
                if(!string.Equals(feature.type,this.geometryType,StringComparison.OrdinalIgnoreCase)) return false;
                this.features.Add(feature);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }//end addFeature
        #endregion
    }//end  EsriFeatureCollection
   
    public class EsriFeature:FeatureBase
    {
        #region "Properties"
        [XmlIgnoreAttribute]
        [JsonIgnore]
        public override string type { get; set; }
        private Object _attributes;
        #endregion
        #region "Constructor"
        public EsriFeature()
        { }
        public EsriFeature(Attributes attr,double x, double y)
        {
            type = "esriGeometryPoint";
            this._attributes = attr;
            this.geometry = new EsriPoint(x,y);
        }
        public EsriFeature(Attributes attr, List<List<List<double>>> rings)
        {
            type = "esriGeometryPolygon";
            this.attributes = attr;
            this.geometry = new EsriPolygon(rings);
        }
        public EsriFeature(JArray jobj, string geometryType)
        {
            type = geometryType;
            FromJson(jobj);
        }
        #endregion
        #region Methods
        protected override bool FromJson(JObject jobj)
        {
            throw new NotImplementedException();
        }
        public Boolean FromJson(JArray jobj)
        {
            try
            {
                foreach (JToken item in jobj)
                {
                    switch (type)
                    {
                        //SSdelineationResult["results"].Where(f=>isFeature(f, out feature)).Select(f => feature).ToList<IFeature>();
                        case "esriGeometryPolygon":
                            var ring = item.SelectToken("geometry.rings");
                            List<List<List<double>>> rings = item.SelectToken("geometry.rings").Select(p => getpolyline(p)).ToList();
                            geometry = new EsriPolygon(rings);
                            attributes = JsonConvert.DeserializeObject<Attributes>(item.SelectToken("attributes").ToString());
                            break;
                        case "esriGeometryPoint":
                            geometry = new EsriPoint((double)item.SelectToken("geometry.x"),(double)item.SelectToken("geometry.y"));
                            attributes = JsonConvert.DeserializeObject<Attributes>(item.SelectToken("attributes").ToString());
                            break;

                        case "esriGeometryPolyline":
                            var path = item.SelectToken("geometry.paths");
                            List<List<List<double>>> paths = item.SelectToken("geometry.paths").Select(p => getpolyline(p)).ToList();
                            geometry = new EsriPolyline(paths);
                            attributes = JsonConvert.DeserializeObject<Attributes>(item.SelectToken("attributes").ToString());
                            break;
                        default:
                            break;
                    }
                }//next item
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }//end FromJson
        public List<List<double>> getpolyline(JToken polyline)
        {
            var x = polyline.Select(p=> new []{(Double)p[0], (Double)p[1]}.ToList<double>()).ToList();

            return x;
        }
        #endregion        
    }//end Features
    
    public class SpatialReference:CoordianteReferenceSystemBase
    {
        #region Properties
        [XmlElement("wkid")]
        [JsonProperty(PropertyName = "wkid")]
        public override object srObject { get; set; }
        #endregion
        #region "Constructor"
        public SpatialReference()
        { }
        public SpatialReference(Int32 wkid)
        {
            this.srObject = wkid;
        }
        #endregion
        #region Methods
        #endregion
    }//end SpatialReference

    public class EsriPoint : GeometryBase
    {
        #region "Properties"
        public double x { get; set; }
        public double y { get; set; }

        #endregion
        #region "Constructor"
        public EsriPoint()
            :base()
        { }
        public EsriPoint(double x, double y)
            : this()
        {
            this.x = x;
            this.y = y;
        }
        #endregion
        #region Methods
        public override List<double> getBoundingBox()
        {
            throw new NotImplementedException();
        }
        #endregion
    }//end EsriPoint

    public class EsriPolygon : GeometryBase
    {
        #region Properties

        [XmlArrayItem("ring")]
        public List<List<List<double>>> rings { get; set; }
        #endregion
        #region Constructor
        public EsriPolygon():base()
        { }
        public EsriPolygon(List<List<List<double>>> rings)
        {
            this.rings = rings;
        }
        #endregion
        #region Methods
        public override List<double> getBoundingBox()
        {
            throw new NotImplementedException();
        }
        #endregion
    }//end EsriPolygon

    public class EsriPolyline : GeometryBase
    {
        #region Properties

        [XmlArrayItem("path")]
        public List<List<List<double>>> paths { get; set; }
        #endregion
        #region Constructor
        public EsriPolyline()
            : base()
        { }
        public EsriPolyline(List<List<List<double>>> paths)
        {
            this.paths = paths;
        }
        #endregion
        #region Methods
        public override List<double> getBoundingBox()
        {
            throw new NotImplementedException();
        }
        #endregion
    }//end EsriPolygon

    #endregion

    #region GeoJson-based FeatureCollection
    
    [XmlInclude(typeof(CRS))]
    [XmlInclude(typeof(Feature))]
    public class FeatureCollection : FeatureCollectionBase
    {
        #region Fields & Properties
        public String type { get; private set; }
        [XmlElement(ElementName = "crs")]
        [JsonProperty(PropertyName = "crs")]
        public override CoordianteReferenceSystemBase crs { get; set; }
        #endregion

        #region Constructors
        public FeatureCollection()
        { type = "FeatureCollection"; }
        public FeatureCollection(Int32 epsgcode)
            : this()
        {
            this.crs = new CRS(epsgcode);
        }
        public FeatureCollection(FeatureBase feature, Int32 epsg)
            : this(epsg)
        {
            features.Add(feature);
        }
        public FeatureCollection(EsriFeatureRecordSet feature)
            :this()
        {            
            crs = (CRS)(feature.crs as SpatialReference);

            feature.features.ForEach(f => this.addFeature(f));
                    }
        #endregion
        #region Explicit operator
        //used for casting
        public static explicit operator FeatureCollection(EsriFeatureRecordSet feature)
        {
            return new FeatureCollection(feature);

        }//end FeatureCollection
        #endregion
        #region Methods
        public override Boolean addFeature(FeatureBase feature)
        {
            try
            {
                if (feature.GetType() == typeof(EsriFeature))
                    this.features.Add((Feature)(feature as EsriFeature));
                else
                    this.features.Add(feature);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }//end addFeature
        #endregion
        
    }//end  EsriFeatureCollection

    public class Feature : FeatureBase
    {
        #region "Properties"
        public override string type { get; set; }
        [JsonProperty("properties")]
        [XmlElement("properties")]
        private Object _attributes;

        public List<double> bbox { get; set; }
        #endregion
        #region "Constructor"
        public Feature()
        { }
        public Feature(Attributes attr, double x, double y)
        {
            type = "Feature";
            this._attributes = attr;
            this.geometry = new Point(x, y);
            bbox = geometry.getBoundingBox();
        }
        public Feature(Object attr, List<List<List<double>>> rings)
        {
            type = "Feature";
            this._attributes = attr;
            this.geometry = new Polygon(rings);
            bbox = this.geometry.getBoundingBox();
        }
        public Feature(JArray jobj, string geometryType)
        {
            type = geometryType;
            FromJson(jobj);
        }
        #endregion
        #region Explicit operator
        //used for casting
        public static explicit operator Feature(EsriFeature feature)
        {
            switch (feature.type)
            {
                case "esriGeometryPoint":
                    EsriPoint pnt = feature.geometry as EsriPoint;
                    return new Feature(feature.attributes, pnt.x, pnt.y);

                case "esriGeometryPolygon":
                    EsriPolygon poly = feature.geometry as EsriPolygon;
                    return new Feature(feature.attributes, poly.rings);
                    
                default:
                    return new Feature();
                   
            }//end switch
            
        }//end FeatureCollection
        #endregion
        #region Methods
        protected override bool FromJson(JObject jobj)
        {
            throw new NotImplementedException();
        }
        public Boolean FromJson(JArray jobj)
        {
            try
            {
                foreach (JToken item in jobj)
                {
                    //TODO: ignore attribute for now.
                    //attributes = item["attributes"];
                    switch (type)
                    {
                        //SSdelineationResult["results"].Where(f=>isFeature(f, out feature)).Select(f => feature).ToList<IFeature>();
                        case "Polygon":
                            var ring = item.SelectToken("geometry.rings");
                            List<List<List<double>>> rings = item.SelectToken("geometry.rings").Select(p => getRing(p)).ToList();
                            geometry = new Polygon(rings);
                            break;
                        case "Point":
                            geometry = new Point((double)item.SelectToken("geometry.y"), (double)item.SelectToken("geometry.x"));
                            break;

                        default:
                            break;
                    }
                }//next item
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }//end FromJson
        public List<List<double>> getRing(JToken ring)
        {
            var x = ring.Select(p => new[] { (Double)p[0], (Double)p[1] }.ToList<double>()).ToList();

            return x;
        }
        #endregion
    }//end Features

    public class CRS : CoordianteReferenceSystemBase
    {
        #region Properties
        public string type { get; set; }
        [XmlElement(ElementName = "properties")]
        [JsonProperty(PropertyName = "properties")]
        public override object srObject { get; set; }
        #endregion
        #region "Constructor"
        public CRS()
        { type = "ESPG"; }
        public CRS(Int32 espgCode)
        {
            dynamic expObj = new System.Dynamic.ExpandoObject();
            expObj.code = espgCode;

            this.srObject = expObj;

        }
        #endregion
        #region Methods
        #endregion
        #region Explicit operator
        //used for casting
        public static explicit operator CRS(SpatialReference sr)
        {
            return new CRS(Convert.ToInt32(sr.srObject));

        }//end FeatureCollection
        #endregion
    }//end SpatialReference

    public class Point : GeometryBase
    {
        #region "Properties"
        public string type { get; set; }
        public List<double> coordinates { get; set; }
               
        #endregion
        #region "Constructor"
        public Point()
            : base()
        { }
        public Point(double x, double y)
            : this()
        {
            type = "Point";
            coordinates = new List<double>{ x, y };
        }
        #endregion
        #region Methods
        public override List<double> getBoundingBox()
        {
            double x = coordinates.FirstOrDefault();
            double y = coordinates.LastOrDefault();
            return new List<Double>() {x, y };
        }
        #endregion

       
    }//end EsriPoint

    public class Polygon : GeometryBase
    {
        #region Properties
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
        #endregion
        #region Constructor
        public Polygon()
            : base()
        { type = "Polygon"; }
        public Polygon(List<List<List<double>>> polygonRings)
            :this()
        {
            this.coordinates = polygonRings;
        }
        #endregion
        #region Methods
        public override List<double> getBoundingBox()
        {

            var bbx = coordinates.Select(polyrings => new
                {

                    xMax = (from c in polyrings select c[0]).Max(),
                    xMin = (from c in polyrings select c[0]).Min(),
                    yMax = (from c in polyrings select c[1]).Max(),
                    yMin = (from c in polyrings select c[1]).Min()

                }).ToList();


            return new List<double> { bbx.Max(x => x.xMax), bbx.Max(y => y.yMax), bbx.Min(x => x.xMin), bbx.Min(y => y.yMin) };
        }
        #endregion
    }//end EsriPolygon

    #endregion

}//end namespace