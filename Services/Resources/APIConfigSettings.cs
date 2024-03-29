﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIM.Resources;

namespace WIM.Services.Resources
{
    public class APIConfigSettings
    {
        public string pathDirectory { get; set; } = "";
        public Dictionary<string, Parameter> Parameters { get; set; }
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
        public Boolean? RequiresAuthentication { get; set; }
    }
}
