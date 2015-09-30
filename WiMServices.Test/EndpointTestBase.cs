using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using OpenRasta;
using OpenRasta.Hosting.InMemory;
using OpenRasta.Web;
using Newtonsoft.Json;
using OpenRasta.Configuration;

//https://www.danielirvine.com/blog/2011/06/08/testing-restful-services-with-openrasta/

namespace WiMServices.Test
{
    public abstract class EndpointTestBase
    {
        #region Properties
        public IConfigurationSource ConfigSource { get; private set; }
        #endregion
        #region Constructor
        public EndpointTestBase(IConfigurationSource config) 
        {
            this.ConfigSource = config;
        }
        #endregion  
        protected T GETRequest<T>(string url)
        {
            using (InMemoryHost host = new InMemoryHost(ConfigSource))
            {
                var request = new InMemoryRequest()
                {
                    Uri = new Uri(url),
                    HttpMethod = "GET"
                };
                // set up your code formats
                request.Entity.ContentType = MediaType.Json;
                request.Entity.Headers["Accept"] = "application/json";

                // send the request and save the resulting response
                var response = host.ProcessRequest(request);
                int statusCode = response.StatusCode;

                // deserialize the content from the response

                if (response.Entity.ContentLength > 0)
                {
                    // you must rewind the stream
                    response.Entity.Stream.Seek(0, SeekOrigin.Begin);

                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamReader streamReader = new StreamReader(response.Entity.Stream, new UTF8Encoding(false, true)))
                    {
                        using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                        {
                            return serializer.Deserialize<T>(jsonTextReader);
                        }//end using
                    }//end using
                }//end if
            }//end using  
            return default(T);
        }
        protected T POSTRequest<T>(string url, T content)
        {
            using (InMemoryHost host = new InMemoryHost(ConfigSource))
            {
                var request = new InMemoryRequest()
                {
                    Uri = new Uri(url),
                    HttpMethod = "POST"
                };
                // set up your code formats
                request.Entity.ContentType = MediaType.Json;
                request.Entity.Headers["Accept"] = "application/json";

                //JsonSerializer serializer1 = new JsonSerializer(typeof(T));
                //serializer1.Serialize(request.Entity.Stream, content);
                //request.Entity.Stream.Seek(0, SeekOrigin.Begin);
                //request.Entity.ContentLength = Request.Entity.Stream.Length;

                // send the request and save the resulting response
                var response = host.ProcessRequest(request);
                int statusCode = response.StatusCode;

                // deserialize the content from the response

                if (response.Entity.ContentLength > 0)
                {
                    // you must rewind the stream, as OpenRasta
                    // won't do this for you
                    response.Entity.Stream.Seek(0, SeekOrigin.Begin);

                    JsonSerializer serializer = new JsonSerializer();
                    using (StreamReader streamReader = new StreamReader(response.Entity.Stream, new UTF8Encoding(false, true)))
                    {
                        using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                        {
                            return serializer.Deserialize<T>(jsonTextReader);
                        }//end using
                    }//end using
                }//end if
            }//end using  
            return default(T);
        }

    }//end class
}//end namespace
