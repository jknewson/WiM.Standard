using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WiM.Resources;

namespace WiM.Services.Resources
{
    public class APIConfigSettings
    {
        public Dictionary<string, Resource> Resources { get; set; }
        public Dictionary<string, Parameter> Parameters { get; set; }
    }
    public class Resource
    {
        public Dictionary<string, string> Description { get; set; }
        public Dictionary<string, Uri> Uris { get; set; }
    }
    public class Parameter
    {
        public string Description { get; set; }
        public Link Link { get; set; }
    }
    public class Uri
    {
        public string Name { get; set; }
        public Dictionary<string, string> Description { get; set; }
    }
}
