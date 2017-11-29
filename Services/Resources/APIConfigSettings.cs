using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WiM.Services.Resources
{
    public class APIConfigSettings
    {
        public Dictionary<string, Resource> Resources { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
    }
    public class Resource
    {
        public string Description { get; set; }
        public Dictionary<string, Uri> Uris { get; set; }
    }
    public class Uri
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
