//------------------------------------------------------------------------------
//----- HyperMediaPipeLineContributors -----------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2016 WiM - USGS

//    authors:  Jeremy Newson          
//  
//   purpose:   Add hypermedia links to the resources if available
//
//discussion:   Hypermedia is an important aspect of REST. It allows you to build services 
//              that decouple client and server to a large extent and allow them to evolve 
//              independently. The representations returned for REST resources contain not 
//              only data, but links to related resources. Thus the design of the representations 
//              is crucial to the design of the overall service. 
//
//     
#region Comments
// 10.13.15 - JKN - Created
#endregion
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

using OpenRasta.Pipeline;
using OpenRasta.Web;

using WiM.Hypermedia;

namespace WiM.PipeLineContributors
{
    public abstract class HypermediaPipelineContributor:IPipelineContributor
    {
        public string BaseURI { get; private set; }
        public string URLQuery { get; private set; }
        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(processOptions).After<KnownStages.IOperationExecution>();
        }
        private PipelineContinuation processOptions(ICommunicationContext context)
        {
            try
            {   
                BaseURI = context.ApplicationBaseUri.AbsoluteUri;
                URLQuery = context.Request.Uri.Query;

                this.Load(context.OperationResult.ResponseResource);
                
            }
            catch (Exception e)
            { }

            return PipelineContinuation.Continue;
        }
        protected virtual void Load(Object obj)
        {
            if (obj.GetType().IsGenericType && obj is IList)
                ((IEnumerable<IHypermedia>)obj).ToList().ForEach(e => e.Links = GetEnumeratedHypermedia(e));
           
            else
                ((IHypermedia)obj).Links = GetReflectedHypermedia((IHypermedia)obj);              
            
        }
        protected abstract List<Link> GetReflectedHypermedia(IHypermedia entity);
        protected abstract List<Link> GetEnumeratedHypermedia(IHypermedia entity);

    }//end class

}//end namespace