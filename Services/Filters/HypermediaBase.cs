//------------------------------------------------------------------------------
//----- HypermediaBase ---------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2017 WIM - USGS

//    authors:  Jeremy K. Newson USGS Web Informatics and Mapping
//              
//  
//   purpose:    Pipline Filter for hypermedia insertion
//
//discussion:   
// https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#action-filters
//     

#region Comments
// 11.01.2017 - jkn Created

#endregion

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using WIM.Hypermedia;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WIM.Resources;
using System.Collections;

namespace WIM.Services.Filters
{
    public abstract class HypermediaBase : IActionFilter
    {
        public string BaseURI { get; private set; }
        public string URLQuery { get; private set; }
        public UrlHelper UrlHelper { get; private set; }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            UrlHelper = new UrlHelper(context);
            BaseURI = context.HttpContext.Request.Host.Value;
            
            URLQuery = context.HttpContext.Request.Path.Value;


            if (URLQuery[0].Equals('/'))
                URLQuery = URLQuery.Remove(0, 1);

            if (context.Result.GetType() != typeof(OkObjectResult)) return;
            this.Load(((OkObjectResult)context.Result).Value);
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //not used
        }
        protected virtual void Load(Object obj)
        {
            try
            {
                if (obj.GetType().IsGenericType && obj.GetType().GetInterfaces().Any(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                    foreach (var item in (IEnumerable<IHypermedia>)obj)
                        item.Links = GetEnumeratedHypermedia(item);                   

                else
                    ((IHypermedia)obj).Links = GetReflectedHypermedia((IHypermedia)obj);
            }
            catch (Exception ex)
            {
                //do nothing
            }

        }

        protected abstract List<Link> GetReflectedHypermedia(IHypermedia entity);
        protected abstract List<Link> GetEnumeratedHypermedia(IHypermedia entity);
    }
}
