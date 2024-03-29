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
//   purpose:   Handle Basic Authentication. !important, Should be used with HTTPS
//
//discussion:   Authentication is the process of determining who you are, while Authorization 
//              evolves around what you are allowed to do, i.e. permissions. Obviously before 
//              you can determine what a user is allowed to do, you need to know who they are, 
//              so when authorisation is required, you must also first authenticate the user in some way.  
//          
//              https://andrewlock.net/introduction-to-authentication-with-asp-net-core/

using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Microsoft.Extensions.DependencyInjection;
using WIM.Resources;


//where all the authentication work is actually done
namespace WIM.Security.Authentication.Basic
{
    public class BasicAuthenticationHandler : AuthenticationHandler<BasicOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<BasicOptions> options, ILoggerFactory logger, 
                                            UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                string authorization = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authorization))
                {
                    return AuthenticateResult.Fail("Authorization failed");
                }
                if (!authorization.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    AuthenticateResult.Fail("Authorization failed");
                }

                var encodedUsernamePassword = authorization.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');
                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);

                var userprincipal = getUserPrincipal(username, password);

                if (userprincipal == null)
                    return AuthenticateResult.Fail("Authorization failed");

                return AuthenticateResult.Success(new AuthenticationTicket(userprincipal, null, Options.AuthenticationScheme));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();
            if(authResult?.Failure != null)
                Response.StatusCode = 401;
        }
        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            await base.HandleForbiddenAsync(properties);
        }

        protected virtual ClaimsPrincipal getUserPrincipal(string username, string password) {

            var agent = Context.RequestServices.GetRequiredService<IAuthenticationAgent>();
            if (agent == null) throw new NullReferenceException($"{typeof(IAuthenticationAgent).Name} not configured in startup.");
            IUser user = agent.AuthenticateUser(username, password);
            
            //set the user
            var claims = new List<Claim> {
                    new Claim(ClaimTypes.Role, user.Role, ClaimValueTypes.String),
                    new Claim(ClaimTypes.PrimarySid, user.ID.ToString(), ClaimValueTypes.Integer),
                    new Claim(ClaimTypes.NameIdentifier, user.Username,ClaimValueTypes.String)
                        };
            var userIdentity = new ClaimsIdentity(claims, Options.AuthenticationScheme);
            return new ClaimsPrincipal(userIdentity);
        }
    }
}
