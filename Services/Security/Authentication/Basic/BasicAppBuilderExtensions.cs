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

using System;
using Microsoft.AspNetCore.Authentication;

namespace WiM.Security.Authentication.Basic
{
    public static class BasicAppBuilderExtensions
    {
        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder)
            => builder.AddBasicAuthentication(BasicDefaults.AuthenticationScheme,"", _ => { });

        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, Action<BasicOptions> configureOptions)
             => builder.AddBasicAuthentication(BasicDefaults.AuthenticationScheme,"", configureOptions);

        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, string authenticationScheme, Action<BasicOptions> configureOptions)
            => builder.AddBasicAuthentication(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddBasicAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BasicOptions> configureOptions)
        {
            return builder.AddScheme<BasicOptions, BasicAuthenticationHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}//end namespace
