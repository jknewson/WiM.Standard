using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using WiM.Resources;
using Newtonsoft.Json;

namespace WiM.Services.Middleware
{
    public class X_Messages
    {
        private readonly RequestDelegate _next;

        public X_Messages(RequestDelegate next)
        {
            _next = next;
        }
        //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware?tabs=aspnetcore2x#per-request-dependencies
        public async Task Invoke(HttpContext httpContext)
        {
            //attach callback to last event that fires before headers are sent.
            httpContext.Response.OnStarting((state) =>
            {
                if (httpContext.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    httpContext.Response.Headers.Add("X-USGSWiM-HostName", Environment.MachineName);
                    //need to attach msg from log here 
                    if (httpContext.Items[X_MessagesExtensions.msgKey] != null)
                    {
                        List<string> msg = ((List<Message>)httpContext.Items[X_MessagesExtensions.msgKey]).GroupBy(g => g.type).Select(gr => gr.Key.ToString() + ": " + string.Join(",", gr.Select(c => c.msg))).ToList();
                        httpContext.Response.Headers.Add("X-USGSWiM-Messages", string.Join(";", msg));
                    }
                }
                return Task.FromResult(0);
            }, null);

            await _next.Invoke(httpContext);            
            
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class X_MessagesExtensions
    {
        public static string msgKey = "wim_msgs";
        public static IApplicationBuilder UseX_Messages(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<X_Messages>();
        }
    }
}
