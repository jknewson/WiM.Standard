using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using WIM.Services.Resources;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Authorization;

namespace WIM.Services.Controllers
{
    [Route("[controller]")]
    public class APISettingsGeneratorController : ControllerBase
    {
        protected readonly IActionDescriptorCollectionProvider _provider;
        public APISettingsGeneratorController(IActionDescriptorCollectionProvider provider)
        {
            _provider = provider;
        }

        [HttpGet]
        public IActionResult GetRoutes()
        {
            try
            {
                APIConfigSettings result = new APIConfigSettings()
                {
                    pathDirectory = "Directory path to linked descriptions",
                    Parameters = _provider.ActionDescriptors.Items.SelectMany(p => p.Parameters.Select(par=>par.Name)).Distinct().ToDictionary(k => k, v => new Parameter() { Description = $"Add description for {v} here, Optional Link shown below - can be removed",
                                                                                                                                                                              Link = new WIM.Resources.Link() { Href="location of reference",
                                                                                                                                                                                                                rel ="Resource URI", method="GET/POST/PUT/DELETE method"} })
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }            
        }        
    }
}
