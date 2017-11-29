//------------------------------------------------------------------------------
//----- Analytics Middleware ---------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Pipeline intermediary to handle analytic requests and responses to the services
//              
//              
//
//discussion:   https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware?tabs=aspnetcore2x
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WiM.Services.Analytics;
using Microsoft.Extensions.Primitives;

namespace WiM.Services.Middleware
{
    public class Analytics
    {
        private readonly RequestDelegate _next;
        public Object[] key { get; set; }

        public Analytics(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext, IAnalyticsAgent agent)
        {
            var request = httpContext.Request;
            
            var parameters = new Dictionary<parameterType, string>();

            parameters.Add(parameterType.path, request.Path.Value);
            parameters.Add(parameterType.operation, request.Method);

            var ip = GetRequestIP(httpContext);
            if (!string.IsNullOrEmpty(ip)) parameters.Add(parameterType.referrer_ip_address, ip );
            if(request.QueryString.HasValue) parameters.Add(parameterType.queryparams, request.QueryString.Value);

            agent.Submit(parameters);
            return _next.Invoke(httpContext);  
        }
        public string GetRequestIP(HttpContext httpContext, bool tryUseXForwardHeader = true )
        {
            string ip = null;

            // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
                ip = SplitCsv(GetHeaderValueAs<string>("X-Forwarded-For",httpContext.Request)).FirstOrDefault();
            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (String.IsNullOrWhiteSpace(ip) && httpContext?.Connection?.RemoteIpAddress != null)
                ip = httpContext.Connection.RemoteIpAddress.ToString();

            if (String.IsNullOrWhiteSpace(ip))
                ip = GetHeaderValueAs<string>("REMOTE_ADDR",httpContext.Request);

            if (String.IsNullOrWhiteSpace(ip))//default
                ip = httpContext.Request.Host.Value;

            return ip;
        }

        public T GetHeaderValueAs<T>(string headerName, HttpRequest request )
        {
            StringValues values;

            if (request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString();   // writes out as Csv when there are multiple.

                if (!String.IsNullOrWhiteSpace(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
            return default(T);
        }
        private List<string> SplitCsv(string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class GA_AnalyticsExtensions
    {
        public static IApplicationBuilder Use_Analytics(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Analytics>();
        }        
    }
   
}
