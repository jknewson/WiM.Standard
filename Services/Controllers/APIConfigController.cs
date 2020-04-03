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
using WIM.Services.Resources;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Abstractions;
using WIM.Resources;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using WIM.Services.Attributes;
using Microsoft.AspNetCore.Authorization;

namespace WIM.Services.Controllers
{
    [Route("[controller]")]
    public class APIConfigController: ControllerBase
    {
        protected readonly IActionDescriptorCollectionProvider _provider;
        protected readonly APIConfigSettings _settings;
        private APIDescriptionAttribute defaultValue = new APIDescriptionAttribute() { type = DescriptionType.e_string, Description = "Description not available" };

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
                Description = ((APIDescriptionAttribute)((ControllerActionDescriptor)k.FirstOrDefault())?
                        .ControllerTypeInfo.GetCustomAttributes(typeof(APIDescriptionAttribute), false)
                            .DefaultIfEmpty(defaultValue).First()).ToDictionary(_settings.pathDirectory),   
                
                Methods = k.Where(m => m.ActionConstraints != null).GroupBy(m => getHttpMethod(m.ActionConstraints.Where(ac => ac.GetType() == typeof(HttpMethodActionConstraint))
                .Cast<HttpMethodActionConstraint>())).Select(x => new ResourceMethod()
                {
                    Type = x.Key,  
                    UriList = x.Where(u => u.AttributeRouteInfo != null).Select(u => {
                        var uristring = u.AttributeRouteInfo.Template.IndexOf(k.Key) == 0 ? u.AttributeRouteInfo.Template.Remove(0, k.Key.Length) : u.AttributeRouteInfo.Template;
                        if (string.IsNullOrEmpty(uristring)) uristring = "/";

                        return new ResourceUri()
                        {
                            Name = u.AttributeRouteInfo.Name ?? u.AttributeRouteInfo.Template,
                            Uri = uristring,
                            RequiresAuthentication = ((ControllerActionDescriptor)u).MethodInfo.GetCustomAttributes(false).OfType<AuthorizeAttribute>().Any(),
                            Description = ((APIDescriptionAttribute)((ControllerActionDescriptor)u).MethodInfo.GetCustomAttributes(typeof(APIDescriptionAttribute), false)
                                    .DefaultIfEmpty(defaultValue).First()).ToDictionary(_settings.pathDirectory),
                            
                            Parameters = u.Parameters.Where(p => p.BindingInfo?.BindingSource.DisplayName != "Body").Select(p => getResourceParams(p)).ToList(),
                            Body = u.Parameters.Where(p => p.BindingInfo?.BindingSource.DisplayName == "Body").Select(p => getResourceParams(p)).ToList()
                        };
                    }).ToList()
                }).ToList()
            }).ToList();
            return Ok(routes);
        }
        #region Helper Methods
        protected virtual string getHttpMethod(IEnumerable<HttpMethodActionConstraint> actionConstraints)
        {
            List<string> methods = actionConstraints.SelectMany(x => x.HttpMethods).ToList();

            return (methods.Count == 1) ? getResourceMethod(methods.FirstOrDefault()) : getResourceMethod("");
        }
        protected String getResourceMethod(string ResourceMethod)
                {

                    switch (ResourceMethod.ToLower())
                    {
                        case "get": return "GET";
                        case "post": return "POST";
                        case "put": return "PUT";
                        case "delete": return "DELETE";
                        case "patch": return "PATCH";
                        default: return "UNSPECIFIED";
                    }

                }
               
        protected virtual ResourceParameter getResourceParams(ParameterDescriptor p)
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
        protected virtual Link getLinkedResource(string propertyname)
        {
            try
            {
                return _settings.Parameters[propertyname]?.Link;
            }
            catch (Exception)
            {
                return null;
            }
        }
        protected virtual string GetTypeName(Type type)
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
