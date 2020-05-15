using System;
using System.Collections.Generic;
using System.Text;

namespace WIM.Storage.AWS
{
    public interface IAWSSettings
    {
        string BucketName { get; set; }
        string Key { get; set; }
        string SecretKey { get; set; }
        /// <summary>
        /// Specified region name, like "us-east-1"
        /// </summary>
        string RegionName { get; set; }
    }
    public class AWSSettings:IAWSSettings
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public string SecretKey { get; set; }
        /// <summary>
        /// Specified region name, like "us-east-1"
        /// </summary>
        public string RegionName { get; set; } = "us-east-1";
    }
}
