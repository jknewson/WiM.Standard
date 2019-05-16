using System;
using Xunit;
using WIM.Storage.AWS;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace WIM.Storage.Test
{
    public class AWSStorageTest
    {
        public AWSSettings stnSettings { get; set; }
        public AWSSettings ssSettings { get; set; }
        public AWSStorageTest()
        {
            //Arrange
            ssSettings = new AWSSettings()
            {
                BucketName = "streamstats-staged-data",
                Key = "***REMOVED***",
                SecretKey = "***REMOVED***",
                RegionName = "***REMOVED***"
            };
            stnSettings = new AWSSettings()
            {

                Key = "**key**",
                SecretKey = "**secretkey**",
                BucketName = "***REMOVED***",
                RegionName = "***REMOVED***"
            };

        }
        
        [Fact]
        public async Task GetAWSObjectTest()
        {

            try
            {                
                var client = new S3Bucket(ssSettings);
                var item = await client.GetObject("xml/StreamStatsAK.xml");

                using (FileStream file = new FileStream("file.xml", FileMode.Create, System.IO.FileAccess.Write))
                    item.CopyTo(file);

                Assert.NotNull(item);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
           


        }
        [Fact]
        public async Task AddAWSObjectTestAsync()
        {

            try
            {
                var client = new S3Bucket(stnSettings);

                Stream fs = File.OpenRead("file.xml");

                await client.AddObject("SITES/SITE_16707/file.xml", fs);                
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }

        [Fact]
        public async Task DeleteAWSObjectTestAsync()
        {

            try
            {
                var client = new S3Bucket(stnSettings);
                await client.DeleteObject("SITES/SITE_16707/file.xml");
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }
        }


        [Fact]
        public async Task ListAWSObjectTestAsync()
        {

            try
            {
                
                var client = new S3Bucket(stnSettings);
                var items = await client.ListObjectsAsync("SITES/SITE_16707");
                var itemkeys = items.Select(o => o.Key);

                //Assert.NotNull(item);
            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }



        }
    }
}
