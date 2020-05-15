//------------------------------------------------------------------------------
//----- JwtBearerAuthenticationBase --------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2019 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   contains base methods for aspnetcore jwp authentication controllers
//
//discussion:   
//  http://www.codeproject.com/Articles/704865/Salted-Password-Hashing-Doing-it-Right
// 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using WIM.Security.Authentication;
using WIM.Services.Controllers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WIM.Services.Attributes;
using WIM.Resources;

namespace WIM.Services.Security.Authentication.JWTBearer
{
    public abstract class JwtBearerAuthenticationBase: Controllers.ControllerBase
    {
        protected IAuthenticationAgent agent { get; set; }
        private string secretkey = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="secretkey">digitally signed secret key (usually passed in from appsettings.json)</param>
        public JwtBearerAuthenticationBase(IAuthenticationAgent agent,string secretkey) : base()
        {
            this.agent = agent;
            this.secretkey = secretkey;
        }

        [AllowAnonymous]
        [HttpPost("/Authenticate", Name = "Authenticate")]
        [APIDescription(type = DescriptionType.e_link, Description = "/Docs/User/authenticate.md")]
        public IActionResult Authenticate([FromBody] UserDTO userDto)
        {
            var user = agent.AuthenticateUser(userDto.UserName, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretkey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Username),
                    new  Claim(ClaimTypes.PrimarySid, user.ID.ToString()),
                    new  Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // return basic user info (without password) and token to store client side
            return Ok(new
            {
                Id = user.ID,
                Username = user.Username,
                Role = user.Role,
                Token = tokenString
            });
        }
    }
}
