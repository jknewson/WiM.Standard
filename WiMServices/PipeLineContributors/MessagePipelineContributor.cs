//------------------------------------------------------------------------------
//----- MessagePipeLineContributors ---------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2016 WiM - USGS

//    authors:  Jeremy Newson          
//  
//   purpose:   Add Message headers
//
//discussion:   
//
//     
#region Comments
// 10.13.15 - JKN - Created
#endregion
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using OpenRasta.Pipeline;
using OpenRasta.Web;

using WiM.Resources;

namespace WiM.PipeLineContributors
{
    public class MessagePipelineContributor:IPipelineContributor
    {
        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(processOptions).After<KnownStages.IOperationExecution>();
        }
        private PipelineContinuation processOptions(ICommunicationContext context)
        {
            try
            {
                context.Response.Headers.Add("USGS-WiM-Service-Messages", context.OperationResult.Description);
            }
            catch (Exception e)
            { }
            return PipelineContinuation.Continue;
        }

    }//end class
}//end namespace