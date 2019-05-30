using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WIM.Security.Authentication;
using WIM.Resources;

namespace WIM.Services.Security.Authentication.JWTBearer
{
    public class JWTBearerAuthenticationEvents: JwtBearerEvents
    {
        public override Task TokenValidated(TokenValidatedContext context)
        {
            IUser user = null;
            try
            {            
                var agent = context.HttpContext.RequestServices.GetRequiredService<IAuthenticationAgent>();
                if (context.Principal.Claims.Any(x=>x.Type == ClaimTypes.PrimarySid))
                {
                    user = agent.GetUserByID(Convert.ToInt32(context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.PrimarySid).Value));
                }
                else if (context.Principal.Claims.Any(x => x.Type == ClaimTypes.NameIdentifier))
                {
                    user = agent.GetUserByUsername(context.Principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
                }//end if
                
                if (user == null)
                {
                    // return unauthorized if user no longer exists
                    context.Fail("Unauthorized");
                }
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
