//------------------------------------------------------------------------------
//----- S3Bucket ---------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2019 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              Jon Baier USGS Web Informatics and Mapping
//              
//              
//  
//   purpose:   Handles File upload methods to AWS S3 bucket.
//
//discussion:   
//              Upload  http://docs.aws.amazon.com/AmazonS3/latest/dev/UploadObjSingleOpNET.html
//              Getting http://docs.aws.amazon.com/AmazonS3/latest/dev/RetrievingObjectUsingNetSDK.html
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WIM.Storage.AWS
{
    public class S3Bucket : IStorage
    {
        #region Properties and Fields
        public string ParentDirectory { get; set; }
        private AWSCredentials Creds { get; set; }
        public RegionEndpoint Region { get; set; }

        #endregion
        #region Constructor
        public S3Bucket(AWSSettings settings)
        {
            this.ParentDirectory = settings.BucketName;
            this.Creds = new BasicAWSCredentials(settings.Key,settings.SecretKey);
            this.Region = RegionEndpoint.GetBySystemName(settings.RegionName);
        }   
        #endregion
        #region Methods       
        public async Task AddObject(string ObjectName, Stream aStream)
        {
            try
            {
                string bucket = Path.GetDirectoryName(ObjectName);
                // simple object put

                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = ParentDirectory,
                    Key = ObjectName,
                    InputStream = aStream
                };
                using (IAmazonS3 client = new AmazonS3Client(Creds,Region))
                {
                    PutObjectResponse response = await client.PutObjectAsync(request);
                    if (response.HttpStatusCode != HttpStatusCode.Accepted ||
                        response.HttpStatusCode != HttpStatusCode.OK)
                        
                        throw new Exception($"{ObjectName} failed to store. {response.ResponseMetadata.ToString()}");
                }//end using
                                    
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                S3Exception(amazonS3Exception);
            }
        }
        public async Task<bool> DeleteObject(string ObjectName)
        {
            try
            {
                using (IAmazonS3 client = new AmazonS3Client())
                {
                    DeleteObjectRequest request = new DeleteObjectRequest()
                    {
                        BucketName = ParentDirectory,
                        Key = ObjectName
                    };
                    DeleteObjectResponse response = await client.DeleteObjectAsync(request);

                }//end using
                return true;
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                S3Exception(amazonS3Exception);
                return false;
            }
        }
        public async Task<Stream> GetObject(string ObjectName)
        {
            //http://theburningmonk.com/2011/06/s3-use-using-block-to-get-the-stream/
            //Because the stream is com­ing from the Ama­zon S3 ser­vice and is fed  
            //in chunks, the code needs to ensure that the con­nec­tion to S3 stays 
            //open until all the data has been received.            
            try
            {
                GetObjectRequest request = new GetObjectRequest()
                                                {
                                                    BucketName = ParentDirectory,
                                                    Key = ObjectName
                                                };

                using (IAmazonS3 client = new AmazonS3Client())
                {
                    using (GetObjectResponse response = await client.GetObjectAsync(request))
                    {
                        //Loads the entire stream before returning
                        if (response.HttpStatusCode == HttpStatusCode.OK)
                        {
                            var data = readStream(response.ResponseStream);

                            return new MemoryStream(data);
                        }
                        throw new AmazonS3Exception(response.HttpStatusCode.ToString());
                        //writes stream to disk for testing purposes
                        //response.WriteResponseStreamToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.test"));

                    }
                }//end using
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                S3Exception(amazonS3Exception);
                return null;
            }

        }//end GetObject
        #endregion
        #region HelperMethods
        private void S3Exception(AmazonS3Exception amazonS3Exception)
        {
            if (amazonS3Exception.ErrorCode != null &&
                (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId")
                || amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
            {
                Console.WriteLine("Please check the provided AWS Credentials.");
            }
            else
            {
                Console.WriteLine("An error occurred with the message '{0}'", amazonS3Exception.Message);
            }
        }
        private byte[] readStream(Stream stream, int initialLength = -1)
        {
            byte[] buffer;
            int read = 0;
            int chunk = 0;
            try
            {
                //initialize variables
                if (initialLength < 1) initialLength = 32768;
                buffer = new byte[initialLength];
                //loop through the stream and copy contents
                while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
                {
                    read += chunk;
                    if (read == buffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        //Check if Done and return
                        if (nextByte == -1) return buffer;

                        byte[] newBuffer = new byte[buffer.Length * 2];
                        Array.Copy(buffer, newBuffer, buffer.Length);
                        newBuffer[read] = (byte)nextByte;
                        buffer = newBuffer;
                        read++;
                    }//end if                
                }//next

                return resizeByte(buffer, read);
            }
            catch (Exception)
            {
                throw;
            }
        }//end ReadStream
        private byte[] resizeByte(byte[] buffer, int size)
        {
            byte[] resizedByte = new byte[size];
            Array.Copy(buffer, resizedByte, size);
            return resizedByte;
        }//end resizeByte
        #endregion
    }
}
