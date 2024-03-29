﻿//------------------------------------------------------------------------------
//----- Service Agent Base ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   The service agentbase is responsible for initiating the service call, 
//              capturing the data that's returned and forwarding the data back to 
//              the requestor.
//
//discussion:   delegated hunting and gathering responsibilities.   
//              Primary responsibility is for http requests
// 
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Xml;
using System.Xml.Serialization;

namespace WIM.Utilities.ServiceAgent
{
    public abstract class ServiceAgentBase
    {
        #region "Events"

        #endregion

        #region Properties & Fields
        private HttpClient client;
        #endregion

        #region Constructors
        public ServiceAgentBase(string BaseUrl)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);
        }
        #endregion

        #region Methods
        protected async Task<T> ExecuteAsync<T>(RequestInfo request)
        {
            return await this.ExecuteAsync<T>(request.RequestURI, request.Content, request.Method);
        }//end ExecuteAsync<T>
        protected async Task<T> ExecuteAsync<T>(string uri, HttpContent data, methodType mtd = methodType.e_GET, contentType overrideResponseDeserializer = contentType.Default)
        {
            try
            {
                HttpResponseMessage response = null;
                T result = default(T);
                response = await this.ExecuteAsync(uri, data, mtd);

                var contenttype = response.Content.Headers.GetValues("Content-Type").FirstOrDefault();
                var stream = await response.Content.ReadAsStreamAsync();
                if (stream != null)
                    result = DeserializeStream<T>(stream, overrideResponseDeserializer == contentType.Default? GetMediaType(contenttype): overrideResponseDeserializer);

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
        }

        protected async void ExecuteAsync(RequestInfo request)
        {
            await ExecuteAsync(request.RequestURI, request.Content, request.Method);
        }//end ExecuteAsync<T>

        protected async Task<HttpResponseMessage> ExecuteAsync(string uri, HttpContent data, methodType mtd = methodType.e_GET)
        {
            try
            {
                HttpResponseMessage response = null;
                switch (mtd)
                {
                    case methodType.e_GET:
                        response = await client.GetAsync(uri);
                        break;
                    case methodType.e_POST:
                        response = await client.PostAsync(uri, data);
                        break;
                    case methodType.e_PUT:
                        response = await client.PutAsync(uri, data);
                        break;
                    case methodType.e_DELETE:
                        response = await client.DeleteAsync(uri);
                        break;
                }//end switch

                if (response == null) throw new Exception("http request invalid");
                //throws an exception if not 200
                response.EnsureSuccessStatusCode();
                return response;               
            }
            catch (HttpRequestException ex)
            {
                throw;
            }
        }

        #endregion
        #region Helper Methods

        protected T DeserializeStream<T>(Stream stream, contentType ContentType)
        {
            using (var sr = new StreamReader(stream))
            {
                switch (ContentType)
                {
                    case contentType.JSON:
                        using (var jsonTextReader = new JsonTextReader(sr))
                        {
                            var serializer = new JsonSerializer();
                            return serializer.Deserialize<T>(jsonTextReader);
                        }//end using

                    case contentType.XML:
                        using (var reader = XmlReader.Create(sr))
                        {
                            var serializer = new XmlSerializer(typeof(T));
                            return (T)serializer.Deserialize(reader);
                        }//end using
                      
                }//end switch
                
                return (T)(object)sr.ReadToEnd();
            }//end using
        }
        protected void Serialize<T>(T value, Stream s)
        {
            using (StreamWriter writer = new StreamWriter(s))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, value);
                jsonWriter.Flush();
            }
        }
        #endregion

        private contentType GetMediaType(string type)
        {
            switch (type)
            {
                case "text/json":
                case "application/json; charset=UTF-8":
                case "application/vnd.geo+json":
                    return contentType.JSON;
                case "text/xml":
                case "application/xml; charset=UTF-8":
                    return contentType.XML;
            }
            return contentType.Default ;
        }

    }//end class ServiceAgentBase

    public class RequestInfo
    {
        public methodType Method { get; set; }
        public string RequestURI { get; set; }
        public string DataType { get; set; }
        public HttpContent Content { get; set; }

        public RequestInfo(string uri, methodType mtd = methodType.e_GET)
        {
            this.RequestURI = uri;
            this.Method = mtd;
        }
        public RequestInfo(string uri, Object data, contentType type = contentType.JSON, methodType mtd = methodType.e_GET)
        {
            this.RequestURI = uri;
            this.Method = mtd;
            switch (type)
            {
                case contentType.JSON:
                    this.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, GetMediaType(type));
                    break;
                case contentType.XML:
                    throw new NotImplementedException("XML not implemented");
            }
        }
        private string GetMediaType(contentType type)
        {
            switch (type)
            {
                case contentType.JSON:
                    return "application/json";
                case contentType.XML:
                    return "application/xml";
            }
            return "application/json";
        }
        
    }
    public enum methodType
    {
        e_GET,
        e_POST,
        e_PUT,
        e_DELETE
    }
    public enum contentType
    {
        JSON,
        XML,
        Default
    }
}
