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

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;

namespace WIM.Services.Messaging
{

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class X_MessagesExtensions
    {
        public static IApplicationBuilder UseX_Messages(this IApplicationBuilder builder, Action<x_MessagesOptions> options)
        {
            var msgOptions = new x_MessagesOptions();
            options(msgOptions);

            return builder.UseMiddleware<X_Messages>(msgOptions);
        }
    }
 
}
