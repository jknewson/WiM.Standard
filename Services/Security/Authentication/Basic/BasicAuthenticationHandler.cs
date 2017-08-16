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
//   purpose:   Handle Basic Authentication. !important, Should be used with HTTPS
//
//discussion:   Authentication is the process of determining who you are, while Authorisation 
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


//where all the authentication work is actually done
namespace WiM.Security.Authentication.Basic
{
    internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        private IBasicUserAgent _agent;
        public BasicAuthenticationHandler(IBasicUserAgent agent) {
            this._agent = agent;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                string authorization = Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authorization))
                {
                    return AuthenticateResult.Skip();
                }
                if (!authorization.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    AuthenticateResult.Skip();
                }

                var encodedUsernamePassword = authorization.Substring("Basic ".Length).Trim();
                Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                int seperatorIndex = usernamePassword.IndexOf(':');
                var username = usernamePassword.Substring(0, seperatorIndex);
                var password = usernamePassword.Substring(seperatorIndex + 1);

                var userprincipal = getUserPrincipal(username, password);

                if (userprincipal == null)
                    return AuthenticateResult.Skip();

                return AuthenticateResult.Success(new AuthenticationTicket(userprincipal, null, Options.AuthenticationScheme));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }
        protected virtual ClaimsPrincipal getUserPrincipal(string username, string password) {

            var user = _agent.GetUserByUsername(username);

            if (user == null || !Cryptography.VerifyPassword(password, user.Salt, user.password))
            {
                return null;
            }

            //set the user
            var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, user.FirstName, ClaimValueTypes.String),
                            new Claim(ClaimTypes.Surname, user.LastName, ClaimValueTypes.String),
                            new Claim(ClaimTypes.Email, user.Email, ClaimValueTypes.String),
                            new Claim(ClaimTypes.Role, user.Role, ClaimValueTypes.String),
                            new Claim(ClaimTypes.Anonymous, user.RoleID.ToString(), ClaimValueTypes.Integer),
                            new Claim(ClaimTypes.PrimarySid, user.ID.ToString(), ClaimValueTypes.Integer),
                            new Claim(ClaimTypes.NameIdentifier, user.Username,ClaimValueTypes.String)
                        };
            var userIdentity = new ClaimsIdentity(claims, Options.AuthenticationScheme);
            return new ClaimsPrincipal(userIdentity);
        }
    }
}
