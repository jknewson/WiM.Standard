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
using Microsoft.AspNetCore.Mvc.Abstractions;
using WiM.Resources;
using System.Collections.Generic;

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
                Name = k.Key,
                Description = getResourceDescription(k.Key),
                Methods = k.GroupBy(m => getResourceMethod(m.RouteValues["Action"])).Select(x => new ResourceMethod()
                {
                    Type = x.Key,
                    UriList = x.Where(u => u.AttributeRouteInfo != null).Select(u => {
                        var uristring = u.AttributeRouteInfo.Template.IndexOf(k.Key) == 0 ? u.AttributeRouteInfo.Template.Remove(0, k.Key.Length) : u.AttributeRouteInfo.Template;
                        if (string.IsNullOrEmpty(uristring)) uristring = "/";

                        return new ResourceUri()
                        {
                            Name = getResourceURIName(k.Key, uristring),
                            Uri = uristring,
                            Description = getResourceDescription(k.Key, uristring),
                            Parameters = u.Parameters.Where(p => p.BindingInfo?.BindingSource.DisplayName != "Body").Select(p => getResourceParams(p)).ToList(),
                            Body = u.Parameters.Where(p => p.BindingInfo?.BindingSource.DisplayName == "Body").Select(p => getResourceParams(p)).ToList()
                        
                        };
                    }).ToList()                    
                }).ToList()
            }).ToList();
            return Ok(routes);
        }
        #region Helper Methods
        protected virtual Dictionary<string, string> getResourceDescription(string ResourceName, string uriname = null) {
            try
            {
            var resource = _settings.Resources[ResourceName];

                if (string.IsNullOrEmpty(uriname)) return resource.Description;
                else return resource.Uris[uriname].Description;
            }
            catch (Exception)
            {
                return new Dictionary<string, string>() { { "string", "Description not available" } };
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
                return _settings.Parameters[propertyname].Description;
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
        protected ResourceParameter getResourceParams(ParameterDescriptor p)
        {
            var parameter = new ResourceParameter()
            {
                Name = p.Name,
                Description = getResourcePropertyDescription(p.Name),
                Type = GetTypeName(p.ParameterType),
                Required = p.BindingInfo == null ? (bool?)true : null,
                Link = getLinkedResource(p.Name)


            };

            return parameter;
        }
        private Link getLinkedResource(string propertyname)
        {
            try
            {
                return _settings.Parameters[propertyname].Link;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected string GetTypeName(Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);
            bool isNullableType = nullableType != null;


            if (isNullableType)
                return string.Format("nullable({0})", nullableType.Name.ToLower());
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
            {
                return string.Format("array({0})", type.GetGenericArguments().Single().Name.ToLower());
            }
            else
                return type.Name;
        }
        #endregion


    }
}
