using System;
using System.Collections.Generic;
using System.Text;

namespace WIM.Storage.AWS
{
    public class AWSSettings
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
