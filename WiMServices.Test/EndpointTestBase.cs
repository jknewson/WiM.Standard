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

namespace WiM.Test
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
        protected T GETRequest<T>(string url, string authenticationHeader = "")
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
                if (!string.IsNullOrEmpty(authenticationHeader)) request.Entity.Headers["Authorization"] = authenticationHeader;

                // send the request and save the resulting response
                var response = host.ProcessRequest(request);
                int statusCode = response.StatusCode;

                // deserialize the content from the response
                return deserialize<T>(response);    
            }//end using 
        }
        protected T POSTRequest<T>(string url, T content,string authenticationHeader = "")
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
                if (!string.IsNullOrEmpty(authenticationHeader)) request.Entity.Headers["Authorization"] = authenticationHeader;
                this.serialize(ref request, content);
                // send the request and save the resulting response
                var response = host.ProcessRequest(request);
                int statusCode = response.StatusCode;

                // deserialize the content from the response
                return deserialize<T>(response);                
            }//end using
        }
        protected T PUTRequest<T>(string url, T content, string authenticationHeader = "")
        {
            using (InMemoryHost host = new InMemoryHost(ConfigSource))
            {
                var request = new InMemoryRequest()
                {
                    Uri = new Uri(url),
                    HttpMethod = "PUT"
                };
                // set up your code formats
                request.Entity.ContentType = MediaType.Json;
                request.Entity.Headers["Accept"] = "application/json";
                if (!string.IsNullOrEmpty(authenticationHeader)) request.Entity.Headers["Authorization"] = authenticationHeader;
                this.serialize(ref request, content);

                // send the request and save the resulting response
                var response = host.ProcessRequest(request);
                int statusCode = response.StatusCode;

                // deserialize the content from the response
                return this.deserialize<T>(response);
            }//end using  
        }
        protected Boolean DELETERequest<T>(string url, string authenticationHeader = "")
        {
            int statusCode = -999;
            using (InMemoryHost host = new InMemoryHost(ConfigSource))
            {
                var request = new InMemoryRequest()
                {
                    Uri = new Uri(url),
                    HttpMethod = "DELETE"
                };
                // set up your code formats
                request.Entity.ContentType = MediaType.Json;
                request.Entity.Headers["Accept"] = "application/json";
                if (!string.IsNullOrEmpty(authenticationHeader)) request.Entity.Headers["Authorization"] = authenticationHeader;

                // send the request and save the resulting response
                var response = host.ProcessRequest(request);
                statusCode = response.StatusCode;

            }//end using  
            return statusCode == 200;
        }


        private void serialize(ref InMemoryRequest request, object content ) {
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(request.Entity.Stream, new UTF8Encoding(false, true))) { CloseOutput = false })
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.None;
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.TypeNameHandling = TypeNameHandling.None;
                serializer.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                serializer.Formatting = Formatting.None;

                serializer.Serialize(jsonTextWriter, content);
                jsonTextWriter.Flush();
                request.Entity.ContentLength = request.Entity.Stream.Length;
                //rewinding the stream.
                request.Entity.Stream.Seek(0, SeekOrigin.Begin);
            }//end using
        
        
        }
        private T deserialize<T>(IResponse response) {
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
            return default(T);
        
        }
    }//end class
}//end namespace
