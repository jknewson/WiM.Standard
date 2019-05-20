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
        public AWSSettings s3Settings { get; set; }
        public AWSStorageTest()
        {
            //Arrange
            s3Settings = new AWSSettings()
            {
                BucketName = "",
                Key = "",
                SecretKey = "",
                RegionName = "***REMOVED***"
            };

        }
        
        [Fact]
        public async Task GetAWSObjectTest()
        {

            try
            {                
                var client = new S3Bucket(s3Settings);
                //s3 is not a path, so / must be used instead of Path.combine
                var item = await client.GetObject("SITES/SITE_16707" + "/" + "46UR1G.jpg");

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
                var client = new S3Bucket(s3Settings);

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
                var client = new S3Bucket(s3Settings);
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
                
                var client = new S3Bucket(s3Settings);
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
