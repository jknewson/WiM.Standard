//------------------------------------------------------------------------------
//----- Authentication -------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WiM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   
//
//discussion:   Authentication is the process of determining who you are, while Authorisation 
//              evolves around what you are allowed to do, i.e. permissions. Obviously before 
//              you can determine what a user is allowed to do, you need to know who they are, 
//              so when authorisation is required, you must also first authenticate the user in some way.  
//          
//              https://andrewlock.net/introduction-to-authentication-with-asp-net-core/

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;

namespace WiM.Security.Authentication.Basic
{
    public static class BasicAppBuilderExtensions
    {
        /// <summary>
        /// Adds the <see cref="BasicAuthenticationMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<BasicAuthenticationMiddleware>();
        }
        /// <summary>
        /// Adds the <see cref="BasicAuthenticationMiddleware"/> middleware to the specified <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to add the middleware to.</param>
        /// <param name="options">A <see cref="BasicAuthenticationOptions"/> that specifies options for the middleware.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, BasicAuthenticationOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return app.UseMiddleware<BasicAuthenticationMiddleware>(Options.Create(options));
        }
    }
}//end namespace
