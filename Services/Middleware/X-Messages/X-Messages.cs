//------------------------------------------------------------------------------
//----- Messaging --------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2019 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   add messages to http response header
//
//discussion:
//    

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WIM.Resources;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace WIM.Services.Messaging
{
    public class X_Messages
    {
        private readonly RequestDelegate _next;
        private readonly x_MessagesOptions _options;


        public X_Messages(RequestDelegate next, x_MessagesOptions options)
        {
            _options = options;
            _next = next;
        }

        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware?tabs=aspnetcore2x#per-request-dependencies
        //https://adamstorr.azurewebsites.net/blog/aspnetcore-exploring-custom-middleware
        public async Task Invoke(HttpContext httpContext)
        {
            //attach callback to last event that fires before headers are sent.
            httpContext.Response.OnStarting((state) =>
            {
            try
            {
                if (!string.IsNullOrEmpty(_options.HostKey))
                    httpContext.Response.Headers.Add(_options.HostKey, System.Environment.MachineName);
                    
                if (X_MessagesDefault.ItemsKey != _options.ItemsKey && httpContext.Items.ContainsKey(X_MessagesDefault.ItemsKey))
                {
                    if (!httpContext.Items.ContainsKey(_options.ItemsKey))
                        httpContext.Items[_options.ItemsKey] = httpContext.Items[X_MessagesDefault.ItemsKey];
                    else
                        ((List<Message>)httpContext.Items[_options.ItemsKey]).AddRange(((List<Message>)httpContext.Items[X_MessagesDefault.ItemsKey]));
                }

                if (httpContext.Items.ContainsKey(_options.ItemsKey) && httpContext.Items[_options.ItemsKey] != null)
                {
                    Dictionary<string, IEnumerable<string>> msg = ((List<Message>)httpContext.Items[_options.ItemsKey])?.GroupBy(g => g.type).ToDictionary(g => g.Key.ToString(), g => g.Select(v => v.msg));
                    httpContext.Response.Headers.Add(_options.messageHeaderString, JsonConvert.SerializeObject(msg));
                }
                }
                catch (Exception ex)
                {
                    //do nothing
                }

                return Task.FromResult(0);
            }, null);

            await _next.Invoke(httpContext);            
            
        }
    }


}
