//------------------------------------------------------------------------------
//----- HttpController ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:   Handles resources through the HTTP uniform interface.
//
//discussion:   Controllers are objects which handle all interaction with resources. 
//              
//
// 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;
using WiM.Services.Resources;
using System;
using System.Linq;

namespace WiM.Services.Controllers
{
    [Route("[controller]")]
    public class APIConfigController: ControllerBase
    {
        private readonly IActionDescriptorCollectionProvider _provider;
        private readonly APIConfigSettings _settings;

        public APIConfigController(IActionDescriptorCollectionProvider provider, IOptions<APIConfigSettings> api_settings)
        {
            _provider = provider;
            _settings = api_settings.Value;
        }
        [HttpGet()]
        public virtual IActionResult GetRoutes()
        {
            var routes = _provider.ActionDescriptors.Items.Where(a => a.RouteValues["Action"] != "GetRoutes").GroupBy(s => ((ControllerActionDescriptor)s).ControllerName).Select(k => 
            new RESTResource()  
            {
                Resource = k.Key,
                Description = getResourceDescription(k.Key),
                Methods = k.GroupBy(m => getResourceMethod(m.RouteValues["Action"])).Select(x => new ResourceMethod()
                {
                    Type = x.Key,
                    UriList = x.Select(u => {
                        var uristring = String.IsNullOrEmpty(u.AttributeRouteInfo.Template.Replace(k.Key, "")) ? "/" : u.AttributeRouteInfo.Template.Replace(k.Key, "");
                        return new ResourceUri() 
                        {
                            Name = getResourceURIName(k.Key, uristring),
                            Uri = uristring,
                            Description = getResourceDescription(k.Key, uristring),
                            Parameters = u.Parameters.Where(p => p.BindingInfo?.BindingSource.DisplayName == "Query").Select(p => new ResourceParameter()
                            {
                                Name = p.Name,
                                Description = getResourcePropertyDescription(p.Name),
                                Type = p.ParameterType.Name
                            }).ToList(),
                            Body = u.Parameters.Where(p => p.BindingInfo?.BindingSource.DisplayName == "Body").Select(p => new ResourceParameter()
                            {
                                Name = p.Name,
                                Description = getResourcePropertyDescription(p.Name),
                                Type = p.ParameterType.Name
                            }).ToList()
                        };
                    }).ToList()                    
                }).ToList()
            }).ToList();
            return Ok(routes);
        }
        #region Helper Methods
        protected virtual String getResourceDescription(string ResourceName, string uriname ="") {
            try
            {
            var resource = _settings.Resources[ResourceName];

                if (string.IsNullOrEmpty(uriname)) return resource.Description;
                else return resource.Uris[uriname].Description;
            }
            catch (Exception)
            {
                return "Description not available";
            }
        }
        protected virtual String getResourceURIName(string ResourceName, string uriname)
        {
            try
            { 
                return _settings.Resources[ResourceName]?.Uris[uriname]?.Name ?? "Name not available";
            }
            catch (Exception)
            {
                return "Name not available";
            }
        }
        protected virtual String getResourcePropertyDescription(string propertyname)
        {
            try
            { 
                return _settings.Parameters[propertyname];
            }
            catch (Exception)
            {
                return "Description not available";
            }
        }
        protected virtual String getResourceMethod(string ResourceMethod)
        {

            switch (ResourceMethod.ToLower())
            {
                case "get": return "GET";
                case "post":return "POST";
                case "put":return "PUT";
                case "delete":return "DELETE";
                case "patch":return "PATCH";
                default: return "UNSPECIFIED";
            }

        }
        #endregion


    }
}
