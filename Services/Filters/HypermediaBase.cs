//https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters#action-filters
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using WiM.Hypermedia;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections;

namespace WiM.Services.Filters
{
    public abstract class HypermediaBase : IActionFilter
    {
        public string BaseURI { get; private set; }
        public string URLQuery { get; private set; }
        public void OnActionExecuted(ActionExecutedContext context)
        {
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
                if (obj.GetType().IsGenericType && obj is IList)
                    ((IEnumerable<IHypermedia>)obj).ToList().ForEach(e => e.Links = GetEnumeratedHypermedia(e));

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
