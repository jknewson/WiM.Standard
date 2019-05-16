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
        /// Specified region name, like "***REMOVED***"
        /// </summary>
        string RegionName { get; set; }
    }
    public class AWSSettings:IAWSSettings
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public string SecretKey { get; set; }
        /// <summary>
        /// Specified region name, like "***REMOVED***"
        /// </summary>
        public string RegionName { get; set; } = "***REMOVED***";
    }
}
