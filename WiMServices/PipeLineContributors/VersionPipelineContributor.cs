//------------------------------------------------------------------------------
//----- PipeLineContributors ---------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2016 WiM - USGS

//    authors:  Jeremy Newson          
//  
//   purpose:   Message headers and properties are treated as HTTP headers
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

namespace WiM.PipeLineContributors
{
    public abstract class VersionPipelineContributor:IPipelineContributor
    {
        public void Initialize(IPipeline pipelineRunner)
        {
            pipelineRunner.Notify(processOptions).Before<KnownStages.IUriMatching>();
        }
        protected abstract PipelineContinuation processOptions(ICommunicationContext context);
    }//end class
}//end namespace