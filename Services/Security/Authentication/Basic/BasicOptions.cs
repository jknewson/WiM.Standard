﻿//------------------------------------------------------------------------------
//----- Authentication -------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

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

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace WIM.Security.Authentication.Basic
{
    public class BasicOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the challenge to put in the "Authenticate" header.
        /// </summary>
        public string AuthenticationScheme { get; set; } = BasicDefaults.AuthenticationScheme;

    }
}
