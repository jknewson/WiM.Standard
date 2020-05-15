using System;
using System.Collections.Generic;
using WIM.Extensions;

namespace WIM.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class APIDescriptionAttribute : Attribute
    {
        public DescriptionType type { get; set; } = DescriptionType.e_string;
        public string Description { get; set; } = "";

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
        [System.ComponentModel.Description("string")]
        e_string,
        [System.ComponentModel.Description("link")]
        e_link
    }
}
