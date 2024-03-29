﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIM.Resources;

namespace WIM.Services.Resources
{
    public class RESTResource
    {
        public string Name { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public List<ResourceMethod> Methods { get; set; }       
    }
    public class ResourceMethod
    {
        public string Type { get; set; }
        public List<ResourceUri> UriList { get; set; }
    }
    public class ResourceUri
    {
        public string Name { get; set; }
        public string Uri { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public Boolean? RequiresAuthentication { get; set; }
        public List<ResourceParameter> Parameters { get; set; }
        public bool ShouldSerializeParameters()
        { return Parameters != null && Parameters.Count > 0; }
        public List<ResourceParameter> Body { get; set; }
        public bool ShouldSerializeBody()
        { return Body != null && Body.Count > 0; }
    }
    public class ResourceParameter
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool? Required { get; set; }
        public Link Link { get; set; }
    }
}
