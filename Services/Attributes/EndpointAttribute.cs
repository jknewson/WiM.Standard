using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using WIM.Extensions;

namespace WIM.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class EndpointAttribute : System.Attribute
    {
        public DescriptionType type { get; set; }
        public string Description { get; set; }

        public Dictionary<string, string> ToDictionary(string descriptionPrecursor ="")
        {
            return new Dictionary<string, string>()
            {
                [this.type.GetDescription()] = this.type == DescriptionType.e_link ? descriptionPrecursor + this.Description : this.Description
            };

        }
    }

    public enum DescriptionType
    {
        [Description("string")]
        e_string,
        [Description("link")]
        e_link
    }
}
